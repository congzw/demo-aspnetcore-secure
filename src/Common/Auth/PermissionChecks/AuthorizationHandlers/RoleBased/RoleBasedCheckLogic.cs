using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Auth.PermissionChecks.AuthorizationHandlers.RoleBased
{
    public class RoleBasedCheckLogic : IPermissionCheckLogicProvider
    {
        public int Order { get; set; }

        protected List<string> MatchedNeedCheckPermissionIds = new List<string>();

        public Task<bool> ShouldCareAsync(PermissionCheckContext permissionCheckContext)
        {
            var registry = permissionCheckContext.ControlPointRegistry;
            var roleBasedRules = registry.RoleBasedRules;
            var permissionIds = roleBasedRules.Keys;
            
            MatchedNeedCheckPermissionIds = permissionCheckContext.MatchedNeedCheckPermissionIds(permissionIds);
            var hasIt = MatchedNeedCheckPermissionIds.Count > 0;
            return hasIt.AsTask();
        }

        public Task<PermissionCheckResult> CheckPermissionAsync(PermissionCheckContext permissionCheckContext)
        {
            //todo
            return PermissionCheckResult.NotSure.WithSource(nameof(RoleBasedCheckLogic)).AsTask();
        }
    }
}