using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NbSites.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            ViewBag.Message = "Enter Login 401";
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

        public IActionResult Forbidden()
        {
            ViewBag.Message = "Enter Forbidden 403";
            return View("Empty");
        }
        
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
