using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using OAuthSample.Demo;

namespace OAuthSample.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        /// <summary>
        /// Logout For Cookie
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Account/SignedOut");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GithubCallback()
        {
            //todo: not work!!!
            //https://localhost:44339/Account/GithubCallback?
            //error=redirect_uri_mismatch
            //&error_description=The+redirect_uri+MUST+match+the+registered+callback+URL+for+this+application.
            //&error_uri=https%3A%2F%2Fdocs.github.com%2Fapps%2Fmanaging-oauth-apps%2Ftroubleshooting-authorization-request-errors%2F%23redirect-uri-mismatch
            //&state=CfDJ8L71t0J--C5KsKQYCy2rgl57qBuf-JKL8wqcrQfQkA6pu3kQfaYWOzX1aL4BVQuWD5DAHsD3L0xFJ5ySsFryHEhehV4NjOi1BRTNfl0ZVV0Aetp-l2QBkzoHLA8vONUJRzx-xlf1E4jzEqA0K0VgkuFC_oCW9nT20GtumwpPxy5_2v6V9pYOJ93ysXGVvjKmUzQJzXasytnpXm7afseiq_k
            
            await Task.CompletedTask;
            var oAuthHelper = OAuthHelper.Instance;
            oAuthHelper.Bags["GithubCallback"] = this.HttpContext.Request.GetDisplayUrl();
            oAuthHelper.Bags["GithubCallbackEnterAt"] = DateTime.Now;
            return RedirectToPage("/Index");
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> GiteeCallback()
        //{
            
        //    //"/Account/GiteeCallback" => AddGitee的实现，并不需要真实存在，但必须配置
        //    await Task.CompletedTask;
        //    var oAuthHelper = OAuthHelper.Instance;
        //    oAuthHelper.Bags["GiteeCallback"] = this.HttpContext.Request.GetDisplayUrl();
        //    oAuthHelper.Bags["GiteeCallbackEnterAt"] = DateTime.Now;
        //    return RedirectToPage("/Index");
        //}
    }
}
