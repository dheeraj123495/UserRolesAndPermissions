using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UserRolesAndPermissions.Models;

namespace UserRolesAndPermissions.Controllers
{
    public class RegisterPageController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        RegisterModel model = new RegisterModel();
        public RegisterPageController(RoleManager<IdentityRole> roleManager, 
                                      UserManager<IdentityUser> userManager, 
                                      IUserStore<IdentityUser> userStore)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
        }

        public IActionResult Index()
        {
            if (!_roleManager.RoleExistsAsync(Roles.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(Roles.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.Role_User)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.Role_Employee)).GetAwaiter().GetResult();
            }
            model = new()
            {
                RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                await _userStore.SetUserNameAsync(user, registerModel.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, registerModel.Email, CancellationToken.None);

                user.Name = registerModel.Name;
                user.City = registerModel.City;
                user.State = registerModel.State;
                user.PhoneNumber = registerModel.PhoneNumber;
                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                    if (!String.IsNullOrEmpty(registerModel.Role))
                    {
                        await _userManager.AddToRoleAsync(user, registerModel.Role);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, Roles.Role_User);
                    }
                }
                return RedirectToAction("Index", "LoginPage");
            }

            return RedirectToAction("Index");
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException("Can't create the instance");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}