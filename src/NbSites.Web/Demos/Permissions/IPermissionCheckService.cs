using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace NbSites.Web.Demos.Permissions
{
    public interface IPermissionCheckService
    {
        //Task HandleRequirementAsync(
        //    AuthorizationHandlerContext context, 
        //    IAuthorizationRequirement requirement, 
        //    ICurrentUserContext userContext, 
        //    PermissionCheckContext permissionCheckContext);
        
        Task<PermissionCheckResult> PermissionCheckAsync(
            ICurrentUserContext userContext,
            PermissionCheckContext permissionCheckContext);
    }
}