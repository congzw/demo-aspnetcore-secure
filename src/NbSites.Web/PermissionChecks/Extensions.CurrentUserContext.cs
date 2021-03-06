﻿namespace NbSites.Web.PermissionChecks
{
    public static class CurrentUserContextExtensions
    {
        public static bool IsLogin(this ICurrentUserContext userContext)
        {
            return !string.IsNullOrWhiteSpace(userContext.User);
        }
    }
}
