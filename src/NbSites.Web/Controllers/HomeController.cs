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
    }
}
