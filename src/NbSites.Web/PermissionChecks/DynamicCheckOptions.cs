namespace NbSites.Web.PermissionChecks
{
    public class DynamicCheckOptions
    {
        public const string SectionName = "PermissionCheck";
        public bool Naked { get; set; } = false;
        public bool DebugHelperEnabled { get; set; } = true;
    }
}