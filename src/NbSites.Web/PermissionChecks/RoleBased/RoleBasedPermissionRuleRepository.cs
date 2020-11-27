using System;
using System.Collections.Generic;

namespace NbSites.Web.PermissionChecks.RoleBased
{
    public interface IRoleBasedPermissionRuleRepository
    {
        IDictionary<string, RoleBasedPermissionRule> GetRules();

        void Save();
    }

    public class RoleBasedPermissionRuleRepository : IRoleBasedPermissionRuleRepository
    {
        public IDictionary<string, RoleBasedPermissionRule> Rules { get; set; } = new Dictionary<string, RoleBasedPermissionRule>(StringComparer.OrdinalIgnoreCase);

        public RoleBasedPermissionRuleRepository()
        {
            //todo: read from source
            Rules.AddOrUpdate(permissionId: KnownPermissionIds.PortalEntry, allowedUsers: "*", allowedRoles: "*");

            Rules.AddOrUpdate(permissionId: KnownPermissionIds.GuestOp, allowedUsers: "*", allowedRoles: "*");
            Rules.AddOrUpdate(permissionId: KnownPermissionIds.LoginOp, allowedUsers: "", allowedRoles: "*");
            Rules.AddOrUpdate(permissionId: KnownPermissionIds.AdminOp, allowedUsers: "", allowedRoles: "Admin,Super");
            Rules.AddOrUpdate(permissionId: KnownPermissionIds.SuperOp, allowedUsers: "", allowedRoles: "Super");
            Rules.AddOrUpdate(permissionId: KnownPermissionIds.LeaderOp, allowedUsers: "", allowedRoles: "Leader");

            Rules.AddOrUpdate(permissionId: KnownPermissionIds.VodOp, allowedUsers: "", allowedRoles: "*");
            Rules.AddOrUpdate(permissionId: KnownPermissionIds.LiveOp, allowedUsers: "*", allowedRoles: "*");

            Rules.AddOrUpdate(permissionId: KnownPermissionIds.UnsureActionA, allowedUsers: "", allowedRoles: "*");
            Rules.AddOrUpdate(permissionId: KnownPermissionIds.DemoOp, allowedUsers: "", allowedRoles: "Admin,Super");
        }

        public IDictionary<string, RoleBasedPermissionRule> GetRules()
        {
            return Rules;
        }

        public void Save()
        {
        }
    }
}