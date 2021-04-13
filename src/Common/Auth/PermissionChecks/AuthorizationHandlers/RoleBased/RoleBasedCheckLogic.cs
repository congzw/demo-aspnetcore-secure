using System.Threading.Tasks;

namespace Common.Auth.PermissionChecks.AuthorizationHandlers.RoleBased
{
    public class RoleBasedCheckLogic : IPermissionCheckLogicProvider
    {
        public int Order { get; set; }

        public Task<bool> ShouldCareAsync(CurrentUserContext userContext, PermissionCheckContext permissionCheckContext)
        {
            //todo
            return false.AsTask();
        }

        public Task<PermissionCheckResult> CheckPermissionAsync(CurrentUserContext userContext, PermissionCheckContext permissionCheckContext)
        {
            //todo
            return PermissionCheckResult.NotSure.AsTask();
        }
    }
}