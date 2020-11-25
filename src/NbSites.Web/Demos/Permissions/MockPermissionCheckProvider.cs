using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace NbSites.Web.Demos.Permissions
{
    public class MockPermissionCheckProvider : IPermissionCheckProvider
    {
        public int Order { get; set; }
        public async Task<PermissionCheckResult> CheckPermissionAsync(ICurrentUserContext userContext, PermissionCheckContext permissionCheckContext)
        {
            if (userContext == null) throw new ArgumentNullException(nameof(userContext));
            if (permissionCheckContext == null) throw new ArgumentNullException(nameof(permissionCheckContext));
            
            if (!KnownFeatureIds.DemoOp.Equals(permissionCheckContext.PermissionId, StringComparison.OrdinalIgnoreCase))
            {
                return PermissionCheckResult.NoCare;
            }

            var allowedOrgIds = userContext.Claims.Where(x => x.Type == KnownClaimTypes.OrgId).Select(x => x.Value).ToList();
            if (allowedOrgIds.Count == 0)
            {
                return PermissionCheckResult.Forbidden.WithMessage("没有任何授权组织");
            }

            if (allowedOrgIds.Contains("*", StringComparer.OrdinalIgnoreCase))
            {
                return PermissionCheckResult.Allowed.WithMessage("所有组织授权");
            }

            var currentOrgId = await TryGetCurrentOrgId(permissionCheckContext);
            if (allowedOrgIds.Contains(currentOrgId, StringComparer.OrdinalIgnoreCase))
            {
                return PermissionCheckResult.Allowed.WithMessage("当前组织已授权: " + currentOrgId);
            }
            return PermissionCheckResult.Forbidden.WithMessage("当前组织没有授权: " + currentOrgId);
        }

        private static async Task<string> TryGetCurrentOrgId(PermissionCheckContext permissionCheckContext)
        {
            //or read currentOrgId from other context
            var httpContext = permissionCheckContext.HttpContext;
            StringValues currentOrgId;
            if (httpContext.Request.Query.TryGetValue("orgId", out currentOrgId))
            {
                return currentOrgId;
            }

            var form = await httpContext.Request.ReadFormAsync();
            if (form.TryGetValue("orgId", out currentOrgId))
            {
                return currentOrgId;
            }

            //or read currentOrgId from other context
            return null;
        }
    }

    public class KnownClaimTypes
    {
        public static string OrgId = "OrgId";
    }
}
