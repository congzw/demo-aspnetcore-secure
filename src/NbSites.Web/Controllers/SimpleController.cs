using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NbSites.Web.PermissionChecks;

namespace NbSites.Web.Controllers
{
    public class SimpleController : Controller
    {
        [AllowAnonymous]
        public IActionResult Escape()
        {
            ViewBag.Message = "Enter Escape";
            return View("Empty");
        }

        [PermissionCheck(PermissionId = KnownPermissionIds.GuestOp)]
        public IActionResult GuestOp()
        {
            ViewBag.Message = "Enter GuestOp";
            return View("Empty");
        }

        [PermissionCheck(PermissionId = KnownPermissionIds.LoginOp)]
        public IActionResult LoginOp()
        {
            ViewBag.Message = "Enter LoginOp";
            return View("Empty");
        }

        [PermissionCheck(PermissionId = KnownPermissionIds.VodOp)]
        public IActionResult VodOp()
        {
            ViewBag.Message = "Enter VodOp";
            return View("Empty");
        }

        [PermissionCheck(PermissionId = KnownPermissionIds.LiveOp)]
        public IActionResult LiveOp()
        {
            ViewBag.Message = "Enter LiveOp";
            return View("Empty");
        }

        [PermissionCheck(PermissionId = KnownPermissionIds.PortalEntry)]
        public IActionResult PortalEntry()
        {
            ViewBag.Message = "Enter PortalEntry";
            return View("Empty");
        }


        public IActionResult Unsure()
        {
            ViewBag.Message = "Enter Unsure";
            return View("Empty");
        }

        public IActionResult Unsure2()
        {
            ViewBag.Message = "Enter Unsure2";
            return View("Empty");
        }

        public IActionResult SpecialAction([FromQuery] SpecialActionArgs args)
        {
            ViewBag.Message = "Enter SpecialAction";
            return View("Empty");
        }

        public class SpecialActionArgs
        {
            public string OrgId { get; set; }
            public string Whatever { get; set; }
        }
    }
}