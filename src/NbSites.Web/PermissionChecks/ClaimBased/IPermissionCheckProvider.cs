using System.Threading.Tasks;

namespace NbSites.Web.PermissionChecks.ClaimBased
{
    public interface IClaimPermissionCheckProvider
    {
        int Order { get; set; }
        Task<PermissionCheckResult> CheckPermissionAsync(ICurrentUserContext userContext, PermissionCheckContext permissionCheckContext);
    }
}