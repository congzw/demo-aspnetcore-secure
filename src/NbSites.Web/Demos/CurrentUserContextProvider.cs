using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace NbSites.Web.Demos
{
    public interface ICurrentUserContextProvider
    {
        CurrentUserContext GetDynamicCheckContext();
    }

    public class CurrentUserContextProvider : ICurrentUserContextProvider
    {
        private readonly IHttpContextAccessor _accessor;

        public CurrentUserContextProvider(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public CurrentUserContext GetDynamicCheckContext()
        {
            var context = _accessor.HttpContext;
            if (context == null)
            {
                return CurrentUserContext.Empty;
            }

            var roleClaims = context.User.Claims.Where(x => x.Type == ClaimTypes.Role).ToList();
            var userName = context.User.Identity.Name;
            var checkFeatureContext = new CurrentUserContext();
            checkFeatureContext.User = userName;
            checkFeatureContext.Roles = roleClaims.Select(x => x.Value).ToList();
            return checkFeatureContext;
        }
    }
}
