using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NbSites.Web.Demos;
using NbSites.Web.PermissionChecks;
using NbSites.Web.PermissionChecks.ResourceBased;

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


        [PermissionCheck(PermissionId = KnownPermissionIds.DemoOp)]
        public IActionResult DemoOp()
        {
            ViewBag.Message = "Enter DemoOp";
            return View("Empty");
        }

        [PermissionCheck(PermissionId = KnownPermissionIds.DemoOp2)]
        public IActionResult DemoOp2([FromQuery] DemoOpArgs args)
        {
            ViewBag.Message = "Enter DemoOp2";
            return View("Empty");
        }

        [PermissionCheck(PermissionId = KnownPermissionIds.DemoOp3)]
        public async Task<IActionResult> DemoOp3([FromServices] IAuthorizationService authorizationService, string orgId = null)
        {
            //todo: read from database 
            var theOrg = new DemoOrg();
            theOrg.OrgId = orgId;
            theOrg.OrtName = "ABC";

            var authorizationResult = await authorizationService
                .AuthorizeAsync(User, theOrg, Operations.Delete);
            
            if (authorizationResult.Succeeded)
            {
                ViewBag.Message = "Enter DemoOp3, Allowed Delete Org: " + theOrg.OrgId;
                return View("Empty");
            }

            if (User.Identity.IsAuthenticated)
            {
                return new ForbidResult();
            }

            return new ChallengeResult();
        }
    }
}