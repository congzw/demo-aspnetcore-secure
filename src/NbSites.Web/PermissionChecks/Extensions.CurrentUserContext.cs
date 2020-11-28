namespace NbSites.Web.PermissionChecks
{
    public static class CurrentUserContextExtensions
    {
        public static bool IsLogin(this ICurrentUserContext userContext)
        {
            return !string.IsNullOrWhiteSpace(userContext.User);
        }
        public static bool HasSuperPower(this ICurrentUserContext userContext)
        {
            return !string.IsNullOrWhiteSpace(userContext.User);
        }
    }
}
