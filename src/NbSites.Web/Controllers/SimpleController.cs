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

        [CheckFeature(Id = ConstFeatureIds.GuestOp)]
        public IActionResult GuestOp()
        {
            ViewBag.Message = "Enter GuestOp";
            return View("Empty");
        }

        [CheckFeature(Id = ConstFeatureIds.LoginOp)]
        public IActionResult LoginOp()
        {
            ViewBag.Message = "Enter LoginOp";
            return View("Empty");
        }

        [CheckFeature(Id = ConstFeatureIds.VodOp)]
        public IActionResult VodOp()
        {
            ViewBag.Message = "Enter VodOp";
            return View("Empty");
        }

        [CheckFeature(Id = ConstFeatureIds.LiveOp)]
        public IActionResult LiveOp()
        {
            ViewBag.Message = "Enter LiveOp";
            return View("Empty");
        }

        [CheckFeature(Id = ConstFeatureIds.PortalEntry)]
        public IActionResult PortalEntry()
        {
            ViewBag.Message = "Enter PortalEntry";
            return View("Empty");
        }
    }
}