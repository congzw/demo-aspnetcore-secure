using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NbSites.Web.Demos;

namespace NbSites.Web.Controllers
{
    [DynamicCheckFeature(Id = ConstFeatureIds.AdminOp)]
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

        [DynamicCheckFeature(Id = ConstFeatureIds.SuperOp)]
        public IActionResult SuperAndAdminOp()
        {
            ViewBag.Message = "Enter SuperAndAdminOp";
            return View("Empty");
        }
        
        [DynamicCheckFeature(Id = ConstFeatureIds.SuperOp, OverrideIds = ConstFeatureIds.AdminOp)]
        public IActionResult SuperOp()
        {
            ViewBag.Message = "Enter SuperOp";
            return View("Empty");
        }
    }
}