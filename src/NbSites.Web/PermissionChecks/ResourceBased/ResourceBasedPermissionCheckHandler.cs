using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NbSites.Web.PermissionChecks.RoleBased;

namespace NbSites.Web.PermissionChecks.ResourceBased
{
    public class ResourceBasedPermissionCheckHandler : AuthorizationHandler<OperationAuthorizationRequirement, object>
    {
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ResourceBasedPermissionCheckHandler> _logger;
        private readonly IPermissionCheckDebugHelper _debugHelper;

        public ResourceBasedPermissionCheckHandler(ICurrentUserContext currentUserContext, 
            IHttpContextAccessor httpContextAccessor, 
            ILogger<ResourceBasedPermissionCheckHandler> logger,
            IPermissionCheckDebugHelper debugHelper)
        {
            _currentUserContext = currentUserContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _debugHelper = debugHelper;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, object resource)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var actionDescriptor = context.GetControllerActionDescriptor(httpContext);
            if (actionDescriptor == null)
            {
                await Task.CompletedTask;
                return;
            }

            _logger.LogInformation(actionDescriptor.DisplayName);

            var providers = httpContext
                .RequestServices
                .GetServices<IResourceBasedCheckLogicProvider>()
                .OrderBy(x => x.Order)
                .ToList();

            if (providers.Count == 0)
            {
                //没由授权逻辑来认领
                return;
            }
            
            var checkResults = new List<PermissionCheckResult>();
            foreach (var logicProvider in providers)
            {
                if (logicProvider.ShouldCare(_currentUserContext, requirement, resource))
                {
                    var moreCheckResult = await logicProvider.CheckPermissionAsync(_currentUserContext, requirement, resource);
                    checkResults.Add(moreCheckResult);
                }
            }

            var permissionCheckResult = checkResults.Combine();

            foreach (var moreCheckResult in checkResults)
            {
                _logger.LogInformation(moreCheckResult.Message);
                _debugHelper.AppendPermissionCheckResults(moreCheckResult);
            }

            if (checkResults.Count > 1)
            {
                _logger.LogInformation(permissionCheckResult.Message);
            }
            
            if (permissionCheckResult.Category == PermissionCheckResultCategory.Allowed)
            {
                context.Succeed(requirement);
            }
        }
    }
}