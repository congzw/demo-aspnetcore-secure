using System.Linq;
using System.Security.Claims;

// ReSharper disable once CheckNamespace
namespace NbSites.Web.PermissionChecks
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetShortTypeName(this Claim claim)
        {
            if (string.IsNullOrWhiteSpace(claim.Type))
            {
                return claim.Type;
            }
            return claim.Type.Split('/').LastOrDefault();
        }
    }
}
