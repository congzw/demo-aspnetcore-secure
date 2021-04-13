using System.Threading.Tasks;

namespace Common.Auth.PermissionChecks.Demo
{
    public class DemoPermissionCheckLogicProvider : IPermissionCheckLogicProvider
    {
        public static bool Allowed = false;

        public int Order { get; set; }

        public Task<bool> ShouldCareAsync(CurrentUserContext userContext, PermissionCheckContext permissionCheckContext)
        {
            return true.AsTask();
        }

        public Task<PermissionCheckResult> CheckPermissionAsync(CurrentUserContext userContext, PermissionCheckContext permissionCheckContext)
        {
            if (Allowed)
            {
                //根据演示flag放行
                return PermissionCheckResult.Allowed.AsTask();
            }
            return PermissionCheckResult.Forbidden.AsTask();
        }
    }
}