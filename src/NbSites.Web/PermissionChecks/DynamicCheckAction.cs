namespace NbSites.Web.PermissionChecks
{
    public class DynamicCheckAction
    {
        public string ActionId { get; set; }
        public string ActionName { get; set; }
        public string PermissionId { get; set; }

        public static DynamicCheckAction Create(string actionId, string permissionId, string actionName = null)
        {
            return new DynamicCheckAction()
            {
                ActionId = actionId,
                ActionName = actionName,
                PermissionId = permissionId
            };
        }
    }
}