using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace NbSites.Web.Demos
{
    public interface ICurrentUserContext
    {
        string User { get; set; }
        List<string> Roles { get; set; }
        List<string> Permissions { get; set; }
        List<Claim> Claims { get; set; }
    }

    public class CurrentUserContext : ICurrentUserContext
    {
        public static string UserKey = ClaimTypes.Name;
        public static string RoleKey = ClaimTypes.Role;
        public static string PermissionKey = "Permission";

        private string _user;
        public string User
        {
            get => _user ??= Claims.Where(x => x.Type == UserKey).Select(x => x.Value).FirstOrDefault();
            set => _user = value;
        }

        private List<string> _roles = null;
        public List<string> Roles
        {
            get => _roles ??= Claims.Where(x => x.Type == RoleKey).Select(x => x.Value).ToList();
            set => _roles = value;
        }

        private List<string> _permissions = null;
        public List<string> Permissions
        {
            get => _permissions ??= Claims.Where(x => x.Type == PermissionKey).Select(x => x.Value).ToList();
            set => _permissions = value;
        }

        public List<Claim> Claims { get; set; } = new List<Claim>();

        public bool IsLogin => !string.IsNullOrWhiteSpace(User);
        public string RolesAsString => string.Join(',', Roles);

        public static CurrentUserContext Empty = new CurrentUserContext();
    }
}