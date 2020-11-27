using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NbSites.Web.PermissionChecks.RoleBased
{
    public class RoleBasedPermissionRuleHandler : AuthorizationHandler<PermissionCheckRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IPermissionRuleActionPool _ruleActionPool;
        private readonly IRoleBasedCheckLogic _roleBasedPermissionRuleLogic;
        private readonly ILogger<RoleBasedPermissionRuleHandler> _logger;
        private readonly IPermissionCheckDebugHelper _debugHelper;
        private readonly IOptionsSnapshot<DynamicCheckOptions> _snapshot;

        public RoleBasedPermissionRuleHandler(IHttpContextAccessor httpContextAccessor, 
            ICurrentUserContext currentUserContext,
            IPermissionRuleActionPool ruleActionPool, 
            IRoleBasedCheckLogic roleBasedPermissionRuleLogic,
            ILogger<RoleBasedPermissionRuleHandler> logger,
            IPermissionCheckDebugHelper debugHelper,
            IOptionsSnapshot<DynamicCheckOptions> snapshot)
        {
            _httpContextAccessor = httpContextAccessor;
            _currentUserContext = currentUserContext;
            _ruleActionPool = ruleActionPool;
            _roleBasedPermissionRuleLogic = roleBasedPermissionRuleLogic;
            _logger = logger;
            _debugHelper = debugHelper;
            _snapshot = snapshot;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionCheckRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var actionDescriptor = context.GetControllerActionDescriptor(httpContext);
            if (actionDescriptor == null)
            {
                await Task.CompletedTask;
                return;
            }

            _logger.LogInformation(actionDescriptor.DisplayName);

            var actionId = actionDescriptor.DisplayName.Split().FirstOrDefault();
            var permissionIds = _ruleActionPool.TryGetPermissionIdsByActionId(actionId);
            if (permissionIds.Count == 0)
            {
                //如果找到ActionId的配置，以此为准，否则以Attribute为准
                permissionIds = context.GetPermissionAttributes(httpContext).Select(x => x.PermissionId).ToList();
            }

            if (permissionIds.Count == 0)
            {
                if (_snapshot.Value.RequiredLoginForUnknown)
                {
                    _logger.LogInformation("没有任何对应规则，根据配置执行登录检测");
                    permissionIds.Add(KnownPermissionIds.LoginOp);
                }
            }
            
            var permissionCheckContext = CreatePermissionCheckContext(context, httpContext, actionDescriptor, permissionIds);
            var permissionRules = _ruleActionPool.TryGetRoleBasedPermissionRules(permissionIds.ToArray());
            var checkResults = _roleBasedPermissionRuleLogic.CheckRules(permissionRules, permissionCheckContext);
           _debugHelper.SetPermissionCheckResults(checkResults.ToArray());
            
            foreach (var permissionCheckResult in checkResults)
            {
                _logger.LogInformation(permissionCheckResult.Message);
            }

            var checkResult = checkResults.Combine();
            if (checkResults.Count > 1)
            {
                _logger.LogInformation(checkResult.Message);
            }

            if (checkResult.Category == PermissionCheckResultCategory.Allowed)
            {
                context.Succeed(requirement);
            }
        }

        private PermissionCheckContext CreatePermissionCheckContext(AuthorizationHandlerContext context, 
            HttpContext httpContext, 
            ActionDescriptor actionDescriptor, 
            IEnumerable<string> permissionIds)
        {
            var permissionCheckContext = new PermissionCheckContext(actionDescriptor, httpContext, _currentUserContext);
            permissionCheckContext.AddCheckPermissionIds(permissionIds?.ToArray());
            return permissionCheckContext;
        }
    }
}
