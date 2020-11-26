using Microsoft.AspNetCore.Authorization;

namespace NbSites.Web.PermissionChecks
{
    public class PermissionCheckRequirement<T> : IAuthorizationRequirement
    {
        public T Resource { get; set; }
    }

    public class PermissionCheckRequirement : PermissionCheckRequirement<object>
    {
    }
}