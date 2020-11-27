using System.Collections.Generic;
using System.Linq;

namespace NbSites.Web.PermissionChecks.RoleBased
{
    public static class RoleBasedExtensions
    {
        public static IList<PermissionCheckResult> CheckRules(this IRoleBasedCheckLogic logic, IEnumerable<RoleBasedPermissionRule> rules, PermissionCheckContext checkContext)
        {
            if (rules == null)
            {
                return new List<PermissionCheckResult>(){ PermissionCheckResult.NotSure };
            }

            var checkResults = rules.Select(x => logic.Check(x, checkContext)).ToList();
            return checkResults;
        }

        public static PermissionCheckResult CheckRulesAsOne(this IRoleBasedCheckLogic logic, IEnumerable<RoleBasedPermissionRule> rules, PermissionCheckContext checkContext)
        {
            return logic.CheckRules(rules, checkContext).Combine();
        }

        public static PermissionCheckResult Combine(this IEnumerable<PermissionCheckResult> permissionCheckResults)
        {
            return PermissionCheckResult.Combine(permissionCheckResults?.ToArray());
        }
        
        public static IDictionary<string, RoleBasedPermissionRule> AddOrUpdate(this IDictionary<string, RoleBasedPermissionRule> rules, RoleBasedPermissionRule theRule)
        {
            rules.AddOrUpdate(theRule.PermissionId, theRule.AllowedUsers, theRule.AllowedRoles);
            return rules;
        }
        public static IDictionary<string, RoleBasedPermissionRule> AddOrUpdate(this IDictionary<string, RoleBasedPermissionRule> rules, string permissionId, string allowedUsers, string allowedRoles)
        {
            var theRule = rules.GetRule(permissionId, true);
            theRule.AllowedUsers = allowedUsers;
            theRule.AllowedRoles = allowedRoles;
            return rules;
        }
        public static RoleBasedPermissionRule GetRule(this IDictionary<string, RoleBasedPermissionRule> rules, string permissionId, bool autoCreate)
        {
            var exist = rules.TryGetValue(permissionId, out var theRule);
            if (!exist)
            {
                if (!autoCreate)
                {
                    return null;
                }
                theRule = new RoleBasedPermissionRule
                {
                    PermissionId = permissionId
                };
                rules[permissionId] = theRule;
            }

            return theRule;
        }
    }
}