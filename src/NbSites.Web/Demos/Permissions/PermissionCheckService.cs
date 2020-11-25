using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace NbSites.Web.Demos.Permissions
{
    public class PermissionCheckService : IPermissionCheckService
    {
        private readonly ILogger<PermissionCheckService> _logger;
        public IList<IPermissionCheckProvider> Providers { get; set; }

        public PermissionCheckService(IEnumerable<IPermissionCheckProvider> providers, ILogger<PermissionCheckService> logger)
        {
            _logger = logger;
            if (providers == null)
            {
                Providers = new List<IPermissionCheckProvider>();
                return;
            }
            Providers = providers.OrderBy(x => x.Order).ToList();
        }

        //public async Task HandleRequirementAsync(AuthorizationHandlerContext context, 
        //    IAuthorizationRequirement requirement, 
        //    ICurrentUserContext userContext,
        //    PermissionCheckContext permissionCheckContext)
        //{
        //    foreach (var provider in Providers)
        //    {
        //        var permissionCheckResult = await provider.CheckPermissionAsync(userContext, permissionCheckContext);
        //        _logger.LogInformation(permissionCheckResult.Message);

        //        if (permissionCheckResult == PermissionCheckResult.NoCare)
        //        {
        //            continue;
        //        }

        //        if (permissionCheckResult == PermissionCheckResult.Allowed)
        //        {
        //            context.Succeed(requirement);
        //            continue;
        //        }

        //        context.Fail();
        //    }
        //}

        public async Task<PermissionCheckResult> PermissionCheckAsync(ICurrentUserContext userContext, PermissionCheckContext permissionCheckContext)
        {
            var permissionCheckResults = new List<PermissionCheckResult>();
            foreach (var provider in Providers)
            {
                var permissionCheckResult = await provider.CheckPermissionAsync(userContext, permissionCheckContext);
                _logger.LogInformation(permissionCheckResult.Message);
                permissionCheckResults.Add(permissionCheckResult);
            }

            var permissionCheckResultCategory = PermissionCheckResult.Combine(permissionCheckResults.ToArray());
            return new PermissionCheckResult(permissionCheckResultCategory, "");
        }
    }
}
