using System.Threading.Tasks;

namespace Common.Auth.PermissionChecks.AuthorizationHandlers.RoleBased
{
    public class RoleBasedCheckLogic : IPermissionCheckLogicProvider
    {
        public int Order { get; set; }

        public Task<bool> ShouldCareAsync(PermissionCheckContext permissionCheckContext)
        {
            return true.AsTask();
        }

        public Task<PermissionCheckResult> CheckPermissionAsync(PermissionCheckContext permissionCheckContext)
        {
            //todo
            return PermissionCheckResult.NotSure.WithSource(nameof(RoleBasedCheckLogic)).AsTask();
        }
    }
}