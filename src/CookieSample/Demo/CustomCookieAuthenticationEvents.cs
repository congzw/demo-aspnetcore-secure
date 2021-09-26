using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CookieSample.Demo
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly ILockUserManager _lockUserManager;

        public CustomCookieAuthenticationEvents(ILockUserManager lockUserManager)
        {
            _lockUserManager = lockUserManager;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var userPrincipal = context.Principal;
            if (userPrincipal == null)
            {
                return;
            }

            var user = userPrincipal.Identity?.Name;
            if (string.IsNullOrWhiteSpace(user))
            {
                return;
            }
            
            //context.RejectPrincipal();
            //context.ReplacePrincipal();
            //context.ShouldRenew
            var isLocked = _lockUserManager.IsLocked(user);
            if (isLocked)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    }
}
