using Common.Auth.PermissionChecks;
using Common.Auth.PermissionChecks.Demo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAndCookie.Controllers
{
    public class DemoController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewBag.Message = "Index => OK";
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
        
        //未注册过的控制点
        public IActionResult NotRegistered()
        {
            ViewBag.Message = "NotRegistered => OK";
            return View("Empty");
        }
        
        [PermissionCheck(PermissionId = DemoConst.PermissionIds.DemoBasedOp)]
        public IActionResult DemoBasedOp()
        {
            ViewBag.Message = "DemoCheckOp => OK";
            return View("Empty");
        }
        
        [PermissionCheck(PermissionId = DemoConst.PermissionIds.GuestOp)]
        public IActionResult GuestOp()
        {
            ViewBag.Message = "GuestOp => OK";
            return View("Empty");
        }

        [PermissionCheck(PermissionId = DemoConst.PermissionIds.LoginOp)]
        public IActionResult LoginOp()
        {
            ViewBag.Message = "LoginOp => OK";
            return View("Empty");
        }

        [PermissionCheck(PermissionId = DemoConst.PermissionIds.AdminOp)]
        public IActionResult AdminOp()
        {
            ViewBag.Message = "AdminOp => OK";
            return View("Empty");
        }

        [PermissionCheck(PermissionId = DemoConst.PermissionIds.SuperOp)]
        public IActionResult SuperOp()
        {
            ViewBag.Message = "SuperOp => OK";
            return View("Empty");
        }

        public IActionResult SmartOp()
        {
            ViewBag.Message = "SmartOp => OK";
            return View("Empty");
        }
    }
}