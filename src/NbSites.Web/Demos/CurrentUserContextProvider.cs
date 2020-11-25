using System.Linq;
using Microsoft.AspNetCore.Http;

namespace NbSites.Web.Demos
{
    public interface ICurrentUserContextProvider
    {
        ICurrentUserContext GetDynamicCheckContext();
    }

    public class CurrentUserContextProvider : ICurrentUserContextProvider
    {
        private readonly IHttpContextAccessor _accessor;

        public CurrentUserContextProvider(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public ICurrentUserContext GetDynamicCheckContext()
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
