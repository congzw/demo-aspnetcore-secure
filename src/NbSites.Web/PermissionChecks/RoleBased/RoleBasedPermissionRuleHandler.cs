using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace NbSites.Web.PermissionChecks.RoleBased
{
    public class RoleBasedPermissionRuleHandler : AuthorizationHandler<PermissionCheckRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IPermissionRuleActionPool _ruleActionPool;
        private readonly IRoleBasedPermissionRuleLogic _roleBasedPermissionRuleLogic;

        public RoleBasedPermissionRuleHandler(IHttpContextAccessor httpContextAccessor, 
            ICurrentUserContext currentUserContext,
            IPermissionRuleActionPool ruleActionPool, 
            IRoleBasedPermissionRuleLogic roleBasedPermissionRuleLogic)
        {
            _httpContextAccessor = httpContextAccessor;
            _currentUserContext = currentUserContext;
            _ruleActionPool = ruleActionPool;
            _roleBasedPermissionRuleLogic = roleBasedPermissionRuleLogic;
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

            var logHelper = PermissionCheckLogHelper.Resolve();
            logHelper.LogInformation(actionDescriptor.DisplayName);

            var actionId = actionDescriptor.DisplayName.Split().FirstOrDefault();
            var permissionIds = _ruleActionPool.TryGetPermissionIdsByActionId(actionId);
            if (permissionIds.Count == 0)
            {
                //如果找到ActionId的配置，以此为准，否则以Attribute为准
                permissionIds = context.GetPermissionAttributes(httpContext).Select(x => x.PermissionId).ToList();
            }

            var permissionCheckContext = CreatePermissionCheckContext(context, httpContext, actionDescriptor, permissionIds);
            var permissionRules = _ruleActionPool.TryGetRoleBasedPermissionRules(permissionIds.ToArray());
            var checkResults = _roleBasedPermissionRuleLogic.CheckRules(permissionRules, permissionCheckContext);

            foreach (var permissionCheckResult in checkResults)
            {
                logHelper.LogInformation(permissionCheckResult.Message);
            }

            var checkResult = checkResults.Combine();
            if (checkResult.Category == PermissionCheckResultCategory.Allowed)
            {
                context.Succeed(requirement);
            }

            if (checkResult.Category == PermissionCheckResultCategory.Forbidden)
            {
                context.Fail();
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
