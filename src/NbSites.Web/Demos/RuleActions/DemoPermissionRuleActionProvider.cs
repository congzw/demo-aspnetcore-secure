using System;
using System.Collections.Generic;
using NbSites.Web.PermissionChecks;
using NbSites.Web.PermissionChecks.RoleBased;

namespace NbSites.Web.Demos
{
    public class DemoPermissionRuleActionProvider : IPermissionRuleActionProvider
    {
        public int Order { get; set; } = 1;

        public void SetRuleActions(IPermissionRuleActionPool ruleActionPool)
        {
            var rules = LoadFromSource(); 
            ruleActionPool.SetRoleBasedPermissionRules(rules);
        }
        
        public IDictionary<string, RoleBasedPermissionRule> Rules { get; set; } = new Dictionary<string, RoleBasedPermissionRule>(StringComparer.OrdinalIgnoreCase);

        private IEnumerable<RoleBasedPermissionRule> LoadFromSource()
        {
            if (Rules.Count == 0)
            {
                Rules.AddOrUpdate(permissionId: KnownPermissionIds.DemoOp, allowedUsers: "", allowedRoles: "Admin,Super");
                Rules.AddOrUpdate(permissionId: KnownPermissionIds.DemoOp2, allowedUsers: "", allowedRoles: "Admin,Super");
                Rules.AddOrUpdate(permissionId: KnownPermissionIds.DemoOp3, allowedUsers: "", allowedRoles: "Admin,Super");
            }
            return Rules.Values;
        }
    }
}
