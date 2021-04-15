using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Auth.PermissionChecks.AuthorizationHandlers.RoleBased
{
    public class RoleBasedCheckLogic : IPermissionCheckLogicProvider
    {
        public int Order { get; set; }

        protected List<RoleBasedRule> MatchedRules = new List<RoleBasedRule>();
        
        public Task<bool> ShouldCareAsync(PermissionCheckContext checkContext)
        {
            var needCheckPermissionIds = checkContext.NeedCheckPermissionIds.ToArray();
            var roleBasedRules = checkContext.ControlPointRegistry.RoleBasedRules;
            MatchedRules = roleBasedRules.GetRoleBasedRules(needCheckPermissionIds);
            var hasIt = MatchedRules.Count > 0;
            return hasIt.AsTask();
        }

        public Task<PermissionCheckResult> CheckPermissionAsync(PermissionCheckContext permissionCheckContext)
        {
            var checkResult = permissionCheckContext.CheckRoleBasedRules(MatchedRules.ToArray());
            return checkResult.AsTask();
        }
    }
}