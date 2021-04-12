using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAndCookie.Controllers
{
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class DemoController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewBag.Message = "Demo Index";
            return View("Empty");
        }

        [AllowAnonymous]
        public IActionResult Anonymous()
        {
            ViewBag.Message = "Anonymous => OK";
            return View($"Empty");
        }

        [Authorize]
        public IActionResult NeedLogin()
        {
            ViewBag.Message = "NeedLogin => OK";
            return View("Empty");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult NeedAdmin()
        {
            ViewBag.Message = "NeedAdmin => OK";
            return View("Empty");
        }

        [Authorize(Roles = "Leader")]
        public IActionResult NeedLeader()
        {
            ViewBag.Message = "NeedLeader => OK";
            return View("Empty");
        }

        [Authorize(Roles = "Admin,Super")]
        public IActionResult NeedAdminSuper()
        {
            ViewBag.Message = "NeedAdmin,Super => OK";
            return View("Empty");
        }

        public IActionResult Fallback()
        {
            ViewBag.Message = "Fallback => OK";
            return View("Empty");
        }
    }
}