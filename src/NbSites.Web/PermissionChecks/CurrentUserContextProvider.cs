﻿using System.Linq;
using Microsoft.AspNetCore.Http;

namespace NbSites.Web.PermissionChecks
{
    public interface ICurrentUserContextProvider
    {
        ICurrentUserContext GetCurrentUserContext();
    }

    public class CurrentUserContextProvider : ICurrentUserContextProvider
    {
        private readonly IHttpContextAccessor _accessor;

        public CurrentUserContextProvider(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public ICurrentUserContext GetCurrentUserContext()
        {
            var context = _accessor.HttpContext;
            if (context == null)
            {
                return CurrentUserContext.Empty;
            }

            var checkFeatureContext = new CurrentUserContext();
            checkFeatureContext.Claims = context.User.Claims.ToList();
            return checkFeatureContext;
        }
    }
}
