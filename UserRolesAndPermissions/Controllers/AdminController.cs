using Microsoft.AspNetCore.Mvc;

namespace UserRolesAndPermissions.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
