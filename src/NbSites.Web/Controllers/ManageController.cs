using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NbSites.Web.PermissionChecks;

namespace NbSites.Web.Controllers
{
    [PermissionCheck(PermissionId = KnownPermissionIds.AdminOp)]
    public class ManageController : Controller
    {
        [AllowAnonymous]
        public IActionResult Escape()
        {
            ViewBag.Message = "Enter Escape";
            return View("Empty");
        }

        public IActionResult AdminOp()
        {
            ViewBag.Message = "Enter AdminOp";
            return View("Empty");
        }

        [PermissionCheck(PermissionId = KnownPermissionIds.SuperOp)]
        public IActionResult SuperOp()
        {
            ViewBag.Message = "Enter SuperAndAdminOp";
            return View("Empty");
        }

        [PermissionCheck(PermissionId = KnownPermissionIds.LeaderOp)]
        public IActionResult LeaderOp()
        {
            ViewBag.Message = "Enter LeaderOp";
            return View("Empty");
        }

        [PermissionCheck(PermissionId = KnownPermissionIds.LeaderOp, OverridePermissionIds = KnownPermissionIds.AdminOp)]
        public IActionResult LeaderOpOverrideAdminOp()
        {
            ViewBag.Message = "Enter LeaderOpOverrideAdminOp";
            return View("Empty");
        }
    }
}