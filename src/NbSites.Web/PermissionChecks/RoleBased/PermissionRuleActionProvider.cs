namespace NbSites.Web.PermissionChecks.RoleBased
{
    public interface IPermissionRuleActionProvider
    {
        /// <summary>
        /// 排序
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// add or update by need
        /// </summary>
        /// <param name="ruleActionPool"></param>
        void SetRuleActions(PermissionRuleActionPool ruleActionPool);
    }

    public class PermissionRuleActionProvider : IPermissionRuleActionProvider
    {
        private readonly IRoleBasedPermissionRuleRepository _ruleRepository;
        private readonly IDynamicCheckActionRepository _actionRepository;

        public PermissionRuleActionProvider(IRoleBasedPermissionRuleRepository ruleRepository, IDynamicCheckActionRepository actionRepository)
        {
            _ruleRepository = ruleRepository;
            _actionRepository = actionRepository;
        }
        
        public int Order { get; set; }

        public void SetRuleActions(PermissionRuleActionPool ruleActionPool)
        {
            var rules = _ruleRepository.GetRules();
            var actions = _actionRepository.GetActions();
            ruleActionPool.SetRoleBasedPermissionRules(rules.Values, true);
            ruleActionPool.SetDynamicCheckActions(actions, true);
        }
    }
}
