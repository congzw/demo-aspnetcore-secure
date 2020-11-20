//namespace NbSites.Web.Demos
//{
//    public class DynamicCheckService
//    {
//        private readonly IDynamicCheckRuleRepository _checkFeatureRuleRepository;

//        public DynamicCheckService(IDynamicCheckRuleRepository checkFeatureRuleRepository)
//        {
//            _checkFeatureRuleRepository = checkFeatureRuleRepository;
//        }

//        public MessageResult IsAllowed(string checkFeatureId, DynamicCheckContext ctx)
//        {
//            var checkFeatureRules = _checkFeatureRuleRepository.GetRules();
//            var messageResult = checkFeatureRules.CheckAllowed(checkFeatureId, ctx);
//            return messageResult;
//        }
//    }
//}