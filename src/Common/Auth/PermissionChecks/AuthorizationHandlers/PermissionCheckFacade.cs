using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Auth.PermissionChecks.AuthorizationHandlers
{
    public class PermissionCheckFacade : AuthorizationHandler<PermissionCheckRequirement>
    {
        private readonly ILogger<PermissionCheckFacade> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPermissionCheckService _permissionCheckService;

        public PermissionCheckFacade(ILogger<PermissionCheckFacade> logger, 
            IHttpContextAccessor httpContextAccessor,
            IPermissionCheckService permissionCheckService)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _permissionCheckService = permissionCheckService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionCheckRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var currentActionId = httpContext.GetCurrentActionId();
            if (currentActionId == null)
            {
                return;
            }
            _logger.LogInformation(currentActionId);

            var userContext = httpContext.GetCurrentUserContext();
            var checkAttributes = httpContext.GetPermissionAttributes();
            var permissionIds = checkAttributes.Select(x => x.PermissionId).ToArray();
            var checkContext = PermissionCheckContext.Create(userContext, requirement, permissionIds);
            var checkResult = await _permissionCheckService.CheckAsync(checkContext);
            switch (checkResult.Category)
            {
                case PermissionCheckResultCategory.Allowed:
                    context.Succeed(requirement);
                    return;
                case PermissionCheckResultCategory.Forbidden:
                    context.Fail();
                    break;
            }
        }
    }
}
