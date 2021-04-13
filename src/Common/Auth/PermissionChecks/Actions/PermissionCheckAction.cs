namespace Common.Auth.PermissionChecks.Actions
{
    public class PermissionCheckAction
    {
        public string ActionId { get; set; }
        public string ActionName { get; set; }
        public string PermissionId { get; set; }

        public static PermissionCheckAction Create(string actionId, string permissionId, string actionName = null)
        {
            return new PermissionCheckAction()
            {
                ActionId = actionId,
                ActionName = actionName,
                PermissionId = permissionId
            };
        }
    }
}
