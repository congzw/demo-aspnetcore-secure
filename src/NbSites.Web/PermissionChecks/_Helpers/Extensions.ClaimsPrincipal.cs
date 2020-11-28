using System.Linq;
using System.Security.Claims;

// ReSharper disable once CheckNamespace
namespace NbSites.Web.PermissionChecks
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// 为了显示方便，截取Uri的最后一截儿
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
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
