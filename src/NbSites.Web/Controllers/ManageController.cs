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
        public IActionResult SuperAndAdminOp()
        {
            ViewBag.Message = "Enter SuperAndAdminOp";
            return View("Empty");
        }

        [PermissionCheck(PermissionId = KnownPermissionIds.SuperOp, OverridePermissionIds = KnownPermissionIds.AdminOp)]
        public IActionResult SuperOp()
        {
            ViewBag.Message = "Enter SuperOp(Override:AdminOp)";
            return View("Empty");
        }
    }
}