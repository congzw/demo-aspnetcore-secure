using System;
using System.Collections.Generic;

namespace NbSites.Web.Demos
{
    public interface ICheckFeatureRuleRepository
    {
        IDictionary<string, CheckFeatureRule> GetCheckFeatureRules();

        void Save();
    }

    public class CheckFeatureRuleRepository : ICheckFeatureRuleRepository
    {
        public IDictionary<string, CheckFeatureRule> CheckFeatureRules { get; set; } = new Dictionary<string, CheckFeatureRule>(StringComparer.OrdinalIgnoreCase);

        public CheckFeatureRuleRepository()
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

        public IDictionary<string, CheckFeatureRule> GetCheckFeatureRules()
        {
            return CheckFeatureRules;
        }

        public void Save()
        {
            
        }
    }
}