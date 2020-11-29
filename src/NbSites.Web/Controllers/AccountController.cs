using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NbSites.Web.PermissionChecks;
using NbSites.Web.PermissionChecks.RoleBased;

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
        
        public async Task<IActionResult> AutoLogin(string returnUrl = null, string role = null, string permission = null)
        {
            //授权部分自己控制，认证部分可参考集成：默认实现，自己实现，第三方系统均可。详细参照： https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-3.1&tabs=visual-studio
            //- ASP.NET Core Identity。
            //- 自己的演示用实现（假定用户是bob，密码检测都ok，claims随意造）。
            //- Azure Active Directory
            //- Azure Active Directory B2C(Azure AD B2C)
            //- IdentityServer4

            const string issuer = "https://mysites.com";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "bob", ClaimValueTypes.String, issuer),
                new Claim(ClaimTypes.DateOfBirth, "1970-06-08", ClaimValueTypes.Date),
                new Claim("OrgId", "123", ClaimValueTypes.String, issuer),
                new Claim("AddAsYouLike", "123456", ClaimValueTypes.String, issuer)
            };

            if (!string.IsNullOrWhiteSpace(role))
            {
                var roleItems = role.MySplit();
                foreach (var roleItem in roleItems)
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleItem, ClaimValueTypes.String, issuer));
                }
            }


            if (!string.IsNullOrWhiteSpace(permission))
            {
                var theItems = permission.MySplit();
                foreach (var theItem in theItems)
                {
                    claims.Add(new Claim("Permission", theItem, ClaimValueTypes.String, issuer));
                }
            }

            var userIdentity = new ClaimsIdentity("ForDemo");
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

        public IActionResult ChangeUnsureSave(
            [FromServices] IRoleBasedPermissionRuleRepository checkFeatureRuleRepository,
            [FromServices] IPermissionRuleActionPoolService poolService,
            [FromServices] IPermissionRuleActionPool pool,
            string mode = null)
        {
            var checkFeatureRules = checkFeatureRuleRepository.GetRules();

            var checkFeatureRule = checkFeatureRules.GetRule(KnownPermissionIds.UnsureOp, false);
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
                    checkFeatureRule.SetNeedUsersOrRoles("", "Admin");
                }
                checkFeatureRuleRepository.Save();
                poolService.RefreshPool(pool);
            }
            return RedirectToAction("Unsure", "Simple");
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
