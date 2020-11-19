namespace NbSites.Web.Demos
{
    public class DynamicCheckFeatureService
    {
        private readonly ICheckFeatureRuleRepository _checkFeatureRuleRepository;

        public DynamicCheckFeatureService(ICheckFeatureRuleRepository checkFeatureRuleRepository)
        {
            _checkFeatureRuleRepository = checkFeatureRuleRepository;
        }

        public MessageResult IsAllowed(string checkFeatureId, CheckFeatureContext ctx)
        {
            var checkFeatureRules = _checkFeatureRuleRepository.GetCheckFeatureRules();
            var messageResult = checkFeatureRules.CheckAllowed(checkFeatureId, ctx);
            return messageResult;
        }
    }
}