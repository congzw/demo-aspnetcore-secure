using System.Threading.Tasks;

namespace NbSites.Web.PermissionChecks
{
    /// <summary>
    /// 扩展点
    /// </summary>
    public interface IPermissionCheckLogicProvider
    {
        int Order { get; set; }
        bool ShouldCare(ICurrentUserContext userContext, PermissionCheckContext permissionCheckContext);
        Task<PermissionCheckResult> CheckPermissionAsync(ICurrentUserContext userContext, PermissionCheckContext permissionCheckContext);
    }
}
