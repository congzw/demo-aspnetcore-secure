using System;
using System.Collections.Generic;

namespace NbSites.Web.Demos
{
    public interface IDynamicCheckRuleRepository
    {
        IDictionary<string, DynamicCheckRule> GetRules();

        void Save();
    }

    public class DynamicCheckRuleRepository : IDynamicCheckRuleRepository
    {
        public IDictionary<string, DynamicCheckRule> CheckFeatureRules { get; set; } = new Dictionary<string, DynamicCheckRule>(StringComparer.OrdinalIgnoreCase);

        public DynamicCheckRuleRepository()
        {
            //todo: read from source
            CheckFeatureRules.AddOrUpdate(id: ConstFeatureIds.PortalEntry, allowedUsers: "*", allowedRoles: "*");

            CheckFeatureRules.AddOrUpdate(id: ConstFeatureIds.GuestOp, allowedUsers: "*", allowedRoles: "*");
            CheckFeatureRules.AddOrUpdate(id: ConstFeatureIds.LoginOp, allowedUsers: "", allowedRoles: "*");
            CheckFeatureRules.AddOrUpdate(id: ConstFeatureIds.AdminOp, allowedUsers: "", allowedRoles: "Admin,Super");
            CheckFeatureRules.AddOrUpdate(id: ConstFeatureIds.SuperOp, allowedUsers: "", allowedRoles: "Super");

            CheckFeatureRules.AddOrUpdate(id: ConstFeatureIds.VodOp, allowedUsers: "", allowedRoles: "*");
            CheckFeatureRules.AddOrUpdate(id: ConstFeatureIds.LiveOp, allowedUsers: "*", allowedRoles: "*");

            CheckFeatureRules.AddOrUpdate(id: ConstFeatureIds.UnsureActionId, allowedUsers: "", allowedRoles: "*");
        }

        public IDictionary<string, DynamicCheckRule> GetRules()
        {
            return CheckFeatureRules;
        }

        public void Save()
        {
            
        }
    }
}