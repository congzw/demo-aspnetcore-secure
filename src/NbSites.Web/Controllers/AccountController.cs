using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NbSites.Web.Demos;

namespace NbSites.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        public IActionResult Login(bool redirected = true)
        {
            ViewBag.Message = redirected ? "Enter Login 401" : "Enter Login";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            ViewBag.Message = "Enter Logout";
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        
        public async Task<IActionResult> AutoLogin(string returnUrl = null, string role = null)
        {
            const string issuer = "https://nbsites.com";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "bob", ClaimValueTypes.String, issuer),
                new Claim("EmployeeId", "123", ClaimValueTypes.String, issuer),
                new Claim(ClaimTypes.DateOfBirth, "1970-06-08", ClaimValueTypes.Date),
                new Claim("BadgeNumber", "123456", ClaimValueTypes.String, issuer)
            };

            if (!string.IsNullOrWhiteSpace(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role, ClaimValueTypes.String, issuer));
            }

            var userIdentity = new ClaimsIdentity("SuperSecureLogin");
            userIdentity.AddClaims(claims);
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                    IsPersistent = false,
                    AllowRefresh = false
                });

            return RedirectToLocal(returnUrl);
        }

        public IActionResult Forbidden(bool redirected = true)
        {
            ViewBag.Message = redirected ? "Enter Forbidden 403" : "Enter Forbidden";
            return View("Empty");
        }

        public IActionResult ChangeUnsure()
        {
            ViewBag.Message = "Enter ChangeUnsure";
            return View();
        }

        public IActionResult ChangeUnsureSave([FromServices] IDynamicCheckRuleRepository checkFeatureRuleRepository, string mode = null)
        {
            var checkFeatureRules = checkFeatureRuleRepository.GetRules();

            var checkFeatureRule = checkFeatureRules.GetRule(KnownFeatureIds.UnsureActionA, false);
            if (checkFeatureRule != null)
            {
                if ("guestAllowed".Equals(mode, StringComparison.OrdinalIgnoreCase))
                {
                    checkFeatureRule.SetNeedGuest();
                }
                if ("loginAllowed".Equals(mode, StringComparison.OrdinalIgnoreCase))
                {
                    checkFeatureRule.SetNeedLogin();
                }
                if ("adminAllowed".Equals(mode, StringComparison.OrdinalIgnoreCase))
                {
                    checkFeatureRule.SetNeedUsersOrRoles(DynamicCheckRuleExpression.None, DynamicCheckRuleExpression.Create("Admin"));
                }
                checkFeatureRuleRepository.Save();
            }
            return RedirectToAction("Unsure", "Home");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
