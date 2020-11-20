using System.Collections.Generic;

namespace NbSites.Web.Demos
{
    public class CurrentUserContext
    {
        public string User { get; set; }
        public List<string> Roles { get; set; } = new List<string>();

        public bool IsLogin => !string.IsNullOrWhiteSpace(User);
        public string RolesAsString => string.Join(',', Roles);

        public static CurrentUserContext Empty = new CurrentUserContext();
    }
}