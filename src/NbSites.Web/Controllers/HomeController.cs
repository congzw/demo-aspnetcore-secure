using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NbSites.Web.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewBag.Message = "Enter Index";
            return View("Empty");
        }
        
        public IActionResult Default()
        {
            ViewBag.Message = "Enter Default";
            return View("Empty");
        }
        
        [Authorize]
        public IActionResult Member()
        {
            ViewBag.Message = "Enter Member";
            return View("Empty");
        }

        [Authorize(policy: "RequireManageRole")]
        public IActionResult Admin()
        {
            ViewBag.Message = "Enter Admin";
            return View("Empty");
        }
    }
}
