using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NbSites.Web.Demos;

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

        [DynamicCheckFeature(Id = KnownFeatureIds.GuestOp)]
        public IActionResult GuestOp()
        {
            ViewBag.Message = "Enter GuestOp";
            return View("Empty");
        }

        [DynamicCheckFeature(Id = KnownFeatureIds.LoginOp)]
        public IActionResult LoginOp()
        {
            ViewBag.Message = "Enter LoginOp";
            return View("Empty");
        }

        [DynamicCheckFeature(Id = KnownFeatureIds.VodOp)]
        public IActionResult VodOp()
        {
            ViewBag.Message = "Enter VodOp";
            return View("Empty");
        }

        [DynamicCheckFeature(Id = KnownFeatureIds.LiveOp)]
        public IActionResult LiveOp()
        {
            ViewBag.Message = "Enter LiveOp";
            return View("Empty");
        }

        [DynamicCheckFeature(Id = KnownFeatureIds.PortalEntry)]
        public IActionResult PortalEntry()
        {
            ViewBag.Message = "Enter PortalEntry";
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