using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Common;
using Common.Auth.JwtAndCookie;
using Common.Auth.JwtAndCookie.Demo;
using Common.Auth.PermissionChecks.Demo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAndCookie.Controllers
{
    [AllowAnonymous]
    public class DemoAccountController : Controller
    {
        private readonly IJwtTokenService _jwtTokenService;

        public DemoAccountController(IJwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }
        
        public async Task<IActionResult> Logout()
        {
            ViewBag.Message = "Enter Logout";
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            //todo: remove this line!
            //为了测试方便，自动补齐Token
            MockClientRequest.Instance.Token = null;

            return RedirectToAction("Index", "Demo");
        }
        
        public async Task<IActionResult> AutoLogin(string returnUrl = null, string role = null, string permission = null)
        {
            var jwtSetting = JwtSetting.Instance;
            var issuer = jwtSetting.Issuer;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "bob", ClaimValueTypes.String, issuer),
                new Claim(ClaimTypes.DateOfBirth, "1970-06-08", ClaimValueTypes.Date),
                new Claim("OrgId", "123", ClaimValueTypes.String, issuer),
                new Claim("AddAsYouLike", "456", ClaimValueTypes.String, issuer)
            };

            if (!string.IsNullOrWhiteSpace(role))
            {
                var roleItems = role.SplitToValues();
                foreach (var roleItem in roleItems)
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleItem, ClaimValueTypes.String, issuer));
                }
            }


            if (!string.IsNullOrWhiteSpace(permission))
            {
                var theItems = permission.SplitToValues();
                foreach (var theItem in theItems)
                {
                    claims.Add(new Claim("Permission", theItem, ClaimValueTypes.String, issuer));
                }
            }
            
            //fix => InvalidOperationException:
            //SignInAsync when principal.Identity.IsAuthenticated is false is not allowed when AuthenticationOptions.RequireAuthenticatedSignIn is true.

            var authType = "MyAuth";
            var userIdentity = new ClaimsIdentity(claims, authType);
            var userPrincipal = new ClaimsPrincipal(userIdentity);
            
            //use cookies
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                    IsPersistent = false,
                    AllowRefresh = false
                });

            //应该返回给客户端，自行处理
            var token = _jwtTokenService.GenerateJsonWebToken(userPrincipal.Claims);
            ViewBag.Token = token;
            
            //todo: remove this line!
            //为了测试方便，自动补齐Token
            //return token to http request client, and set header by client!
            MockClientRequest.Instance.Token = token;

            return RedirectToLocal(returnUrl);
        }

        //For 401
        public IActionResult Login(bool redirected = true)
        {
            ViewBag.Message = redirected ? "Enter Login 401" : "Enter Login";
            return View("Empty");
        }

        //For 403
        public IActionResult Forbidden(bool redirected = true)
        {
            var msg = redirected ? "Enter Forbidden 403" : "Enter Forbidden";
            ViewBag.Message = msg + " => K.O.";
            return View("Empty");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Demo");
        }

        public IActionResult SetDemoBased(bool allowed)
        {
            DemoPermissionCheckLogicProvider.Allowed = allowed;
            ViewBag.Message = "DemoPermissionCheckLogicProvider.Allowed => " + DemoPermissionCheckLogicProvider.Allowed;
            return View("Empty");
        }
    }
}
