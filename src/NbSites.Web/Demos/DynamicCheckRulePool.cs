using System.Linq;

namespace NbSites.Web.Demos
{
    public class DynamicCheckRulePool
    {
        private readonly IDynamicCheckActionRepository _actionRepository;
        private readonly IDynamicCheckRuleRepository _ruleRepository;

        public DynamicCheckRulePool(IDynamicCheckActionRepository actionRepository, IDynamicCheckRuleRepository ruleRepository)
        {
            _actionRepository = actionRepository;
            _ruleRepository = ruleRepository;
        }
        
        public MessageResult IsAllowed(string checkFeatureId, CurrentUserContext ctx)
        {
            var checkFeatureRules = _ruleRepository.GetRules();
            var messageResult = checkFeatureRules.CheckAllowed(checkFeatureId, ctx);
            return messageResult;
        }

        public DynamicCheckRule TryGetRuleByActionId(string actionId)
        {
            //todo: cache check result?
            var dictionary = _actionRepository.GetActions().ToDictionary(x => x.ActionId);
            if (!dictionary.TryGetValue(actionId, out var theAction))
            {
                return null;
            }
            return TryGetRuleByFeatureId(theAction.CheckFeatureId);
        }

        public DynamicCheckRule TryGetRuleByFeatureId(string featureId)
        {
            //todo: cache check result?
            var checkRules = _ruleRepository.GetRules();
            checkRules.TryGetValue(featureId, out var theRule);
            return theRule;
        }
    }
}