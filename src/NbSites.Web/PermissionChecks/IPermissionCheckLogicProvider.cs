using System.Threading.Tasks;

namespace NbSites.Web.PermissionChecks
{
    /// <summary>
    /// 扩展点: 检测逻辑的提供者接口
    /// </summary>
    public interface IPermissionCheckLogicProvider
    {
        int Order { get; set; }
        Task<bool> ShouldCareAsync(ICurrentUserContext userContext, PermissionCheckContext permissionCheckContext);
        Task<PermissionCheckResult> CheckPermissionAsync(ICurrentUserContext userContext, PermissionCheckContext permissionCheckContext);
    }
}
