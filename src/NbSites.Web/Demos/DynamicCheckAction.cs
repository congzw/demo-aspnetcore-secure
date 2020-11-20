namespace NbSites.Web.Demos
{
    public class DynamicCheckAction
    {
        public string ActionId { get; set; }
        public string ActionName { get; set; }
        public string CheckFeatureId { get; set; }

        public static DynamicCheckAction Create(string actionId, string checkFeatureId, string actionName = null)
        {
            return new DynamicCheckAction()
            {
                ActionId = actionId,
                ActionName = actionName,
                CheckFeatureId = checkFeatureId
            };
        }
    }
}