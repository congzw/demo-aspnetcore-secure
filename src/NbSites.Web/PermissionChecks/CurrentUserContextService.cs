using System.Linq;
using Microsoft.AspNetCore.Http;

namespace NbSites.Web.PermissionChecks
{
    public interface ICurrentUserContextService
    {
        ICurrentUserContext GetCurrentUserContext();
    }

    public class CurrentUserContextService : ICurrentUserContextService
    {
        private readonly IHttpContextAccessor _accessor;

        public CurrentUserContextService(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public ICurrentUserContext GetCurrentUserContext()
        {
            var context = _accessor.HttpContext;
            if (context == null)
            {
                return CurrentUserContext.Empty;
            }

            var checkFeatureContext = new CurrentUserContext();
            checkFeatureContext.Claims = context.User.Claims.ToList();
            return checkFeatureContext;
        }
    }
}
