using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace NbSites.Web.PermissionChecks.ResourceBased
{
    //public class ResourceBasedPermissionCheckRequirement<T> : IAuthorizationRequirement
    //{
    //    public T Resource { get; set; }
    //}

    //public class ResourceBasedPermissionCheckRequirement : ResourceBasedPermissionCheckRequirement<object>
    //{
    //}

    public static class Operations
    {
        public static OperationAuthorizationRequirement Create =
            new OperationAuthorizationRequirement { Name = nameof(Create) };
        public static OperationAuthorizationRequirement Read =
            new OperationAuthorizationRequirement { Name = nameof(Read) };
        public static OperationAuthorizationRequirement Update =
            new OperationAuthorizationRequirement { Name = nameof(Update) };
        public static OperationAuthorizationRequirement Delete =
            new OperationAuthorizationRequirement { Name = nameof(Delete) };
    }

    /// <summary>
    /// 扩展点
    /// </summary>
    public interface IResourceBasedCheckLogicProvider
    {
        int Order { get; set; }
        bool ShouldCare(ICurrentUserContext userContext, OperationAuthorizationRequirement requirement, object resource);
        Task<PermissionCheckResult> CheckPermissionAsync(ICurrentUserContext userContext, OperationAuthorizationRequirement requirement, object resource);
    }
}
