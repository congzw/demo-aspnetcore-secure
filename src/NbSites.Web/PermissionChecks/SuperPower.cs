using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace NbSites.Web.PermissionChecks
{
    public interface ISuperPowerProvider
    {
        int Order { get; set; }
        Task<bool> HasSuperPowerAsync(AuthorizationHandlerContext context, HttpContext httpContext, ICurrentUserContext userContext);
    }

    public class SuperPowerCheck
    {
        private readonly IList<ISuperPowerProvider> _superPowerProviders;

        public SuperPowerCheck(IEnumerable<ISuperPowerProvider> superPowerProviders)
        {
            _superPowerProviders = superPowerProviders.ToList();
        }

        public async Task<bool> HasSuperPowerAsync(AuthorizationHandlerContext context, HttpContext httpContext, ICurrentUserContext userContext)
        {
            var isSuperRole = userContext.Roles.MyContains("Super");
            if (isSuperRole)
            {
                return true;
            }

            var providers = httpContext
                .RequestServices
                .GetServices<ISuperPowerProvider>()
                .OrderBy(x => x.Order)
                .ToList();

            foreach (var provider in providers)
            {
                if (await provider.HasSuperPowerAsync(context, httpContext, userContext))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
