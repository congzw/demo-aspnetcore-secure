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
        private readonly IPermissionRuleActionPool _ruleActionPool;
        private readonly IRoleBasedPermissionRuleLogic _roleBasedPermissionRuleLogic;

        public RoleBasedPermissionRuleHandler(IHttpContextAccessor httpContextAccessor, IPermissionRuleActionPool ruleActionPool, IRoleBasedPermissionRuleLogic roleBasedPermissionRuleLogic)
        {
            _httpContextAccessor = httpContextAccessor;
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
            var roleBasedPermissionRules = _ruleActionPool.TryGetRoleBasedPermissionRulesByActionId(actionId);

            var permissionIds = roleBasedPermissionRules.Count > 0 ?
                //如果找到ActionId的配置，以此为准
                roleBasedPermissionRules.Select(x => x.PermissionId).ToArray() : 
                context.GetPermissionAttributes(httpContext).Select(x => x.PermissionId).ToArray();

            var permissionCheckContext = CreatePermissionCheckContext(context, httpContext, actionDescriptor, permissionIds);
            var checkResult = _roleBasedPermissionRuleLogic.CheckRules(roleBasedPermissionRules, permissionCheckContext);

            if (checkResult.Category == PermissionCheckResultCategory.Allowed)
            {
                context.Succeed(requirement);
            }
        }
        
        private PermissionCheckContext CreatePermissionCheckContext(AuthorizationHandlerContext context, HttpContext httpContext, ActionDescriptor actionDescriptor, IEnumerable<string> permissionIds)
        {
            var permissionCheckContext = new PermissionCheckContext();
            permissionCheckContext.HttpContext = httpContext;
            permissionCheckContext.ActionDescriptor = actionDescriptor;
            permissionCheckContext.AddCheckPermissionIds(permissionIds?.ToArray());

            return permissionCheckContext;
        }
    }
}
