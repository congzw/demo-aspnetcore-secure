using System.Linq;

namespace NbSites.Web.PermissionChecks.RoleBased
{
    public class MockHelper
    {
        public static CurrentUserContext CreateUserContext(string user, params string[] roles)
        {
            var userContext = new CurrentUserContext();
            userContext.User = user;
            userContext.Roles = roles.ToList();
            return userContext;
        }

        public static PermissionCheckContext CreatePermissionCheckContext(string permissionIdsValue, string user, params string[] roles)
        {
            var context = new PermissionCheckContext();
            context.AddCheckPermissionIdsValue(permissionIdsValue);
            context.CurrentUserContext = CreateUserContext(user, roles);
            return context;
        }
    }
}
