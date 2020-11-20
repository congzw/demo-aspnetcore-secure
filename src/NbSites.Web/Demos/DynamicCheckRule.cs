namespace NbSites.Web.Demos
{
    public class DynamicCheckRule
    {
        public string CheckFeatureId { get; set; }
        public string AllowedUsers { get; set; }
        public string AllowedRoles { get; set; }

        public static DynamicCheckRule Create(string id)
        {
            return new DynamicCheckRule() { CheckFeatureId = id };
        }
    }
}
