using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NbSites.Web.Demos;

namespace NbSites.Web.Controllers
{
    [DynamicCheckFeature(Id = KnownFeatureIds.AdminOp)]
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

        [DynamicCheckFeature(Id = KnownFeatureIds.SuperOp)]
        public IActionResult SuperAndAdminOp()
        {
            ViewBag.Message = "Enter SuperAndAdminOp";
            return View("Empty");
        }
        
        [DynamicCheckFeature(Id = KnownFeatureIds.SuperOp, OverrideIds = KnownFeatureIds.AdminOp)]
        public IActionResult SuperOp()
        {
            ViewBag.Message = "Enter SuperOp";
            return View("Empty");
        }
    }
}