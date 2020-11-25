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
            CheckFeatureRules.AddOrUpdate(id: KnownFeatureIds.PortalEntry, allowedUsers: "*", allowedRoles: "*");

            CheckFeatureRules.AddOrUpdate(id: KnownFeatureIds.GuestOp, allowedUsers: "*", allowedRoles: "*");
            CheckFeatureRules.AddOrUpdate(id: KnownFeatureIds.LoginOp, allowedUsers: "", allowedRoles: "*");
            CheckFeatureRules.AddOrUpdate(id: KnownFeatureIds.AdminOp, allowedUsers: "", allowedRoles: "Admin,Super");
            CheckFeatureRules.AddOrUpdate(id: KnownFeatureIds.SuperOp, allowedUsers: "", allowedRoles: "Super");

            CheckFeatureRules.AddOrUpdate(id: KnownFeatureIds.VodOp, allowedUsers: "", allowedRoles: "*");
            CheckFeatureRules.AddOrUpdate(id: KnownFeatureIds.LiveOp, allowedUsers: "*", allowedRoles: "*");

            CheckFeatureRules.AddOrUpdate(id: KnownFeatureIds.UnsureActionA, allowedUsers: "*", allowedRoles: "*");
            CheckFeatureRules.AddOrUpdate(id: KnownFeatureIds.DemoOp, allowedUsers: "", allowedRoles: "Admin,Super");
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