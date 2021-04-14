using System.Threading.Tasks;

namespace Common.Auth.PermissionChecks.Demo
{
    //演示一个硬编码的扩展
    public class DemoBasedLogic : IPermissionCheckLogicProvider
    {
        public static bool Allowed = false;
        
        public int Order { get; set; }

        public Task<bool> ShouldCareAsync(PermissionCheckContext permissionCheckContext)
        {
            return permissionCheckContext.MatchPermissionId(DemoConst.PermissionIds.DemoBasedOp).AsTask();
        }

        public Task<PermissionCheckResult> CheckPermissionAsync(PermissionCheckContext permissionCheckContext)
        {
            if (Allowed)
            {
                //根据演示flag放行
                return PermissionCheckResult.Allowed.WithSource(nameof(DemoBasedLogic)).AsTask();
            }
            return PermissionCheckResult.Forbidden.WithSource(nameof(DemoBasedLogic)).AsTask();
        }
    }
}