using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NbSites.Web.PermissionChecks.RoleBased
{
    public interface IPermissionRuleActionPool
    {
        void SetRoleBasedPermissionRules(IEnumerable<RoleBasedPermissionRule> rules, bool replaceExist = true);
        void SetDynamicCheckActions(IEnumerable<DynamicCheckAction> actions, bool replaceExist = true);
        IList<RoleBasedPermissionRule> TryGetRoleBasedPermissionRulesByActionId(string actionId);
    }

    public class PermissionRuleActionPool : IPermissionRuleActionPool
    {
        public IDictionary<string, RoleBasedPermissionRule> RoleBasedPermissionRules { get; set; } = new ConcurrentDictionary<string, RoleBasedPermissionRule>();
        public List<DynamicCheckAction> DynamicCheckActions { get; set; } = new List<DynamicCheckAction>();
        public IDictionary<string, List<RoleBasedPermissionRule>> CachedActionRules { get; set; } = new ConcurrentDictionary<string, List<RoleBasedPermissionRule>>();

        public void SetRoleBasedPermissionRules(IEnumerable<RoleBasedPermissionRule> rules, bool replaceExist = true)
        {
            if (rules == null)
            {
                return;
            }
            foreach (var rule in rules)
            {
                if (RoleBasedPermissionRules.TryGetValue(rule.PermissionId, out var theRule))
                {
                    if (replaceExist)
                    {
                        RoleBasedPermissionRules.AddOrUpdate(rule);
                    }
                }
            }
        }

        public void SetDynamicCheckActions(IEnumerable<DynamicCheckAction> actions, bool replaceExist = true)
        {
            if (actions == null)
            {
                return;
            }

            foreach (var action in actions)
            {
                var theOne = DynamicCheckActions.FirstOrDefault(x => 
                    x.ActionId.MyEquals(action.ActionId) && 
                    x.PermissionId.MyEquals(action.PermissionId));

                if (theOne == null)
                {
                    DynamicCheckActions.Add(action);
                }
                else
                {
                    if (replaceExist)
                    {
                        theOne.PermissionId = action.PermissionId;
                        theOne.ActionId = action.ActionId;
                        theOne.ActionName = action.ActionName;
                    }
                }
            }
        }

        public IList<RoleBasedPermissionRule> TryGetRoleBasedPermissionRulesByActionId(string actionId)
        {
            if (CachedActionRules.TryGetValue(actionId, out var rules))
            {
                return rules;
            }

            rules = new List<RoleBasedPermissionRule>();
            var permissionIds = DynamicCheckActions.Where(x => x.ActionId.MyEquals(actionId)).Select(x => x.PermissionId).ToList();
            foreach (var permissionId in permissionIds)
            {
                if (RoleBasedPermissionRules.TryGetValue(permissionId, out var theRule))
                {
                    rules.Add(theRule);
                }
            }

            CachedActionRules[actionId] = rules;
            return rules;
        }
    }
}
