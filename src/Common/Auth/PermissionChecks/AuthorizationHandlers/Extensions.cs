using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Common.Auth.PermissionChecks.AuthorizationHandlers
{
    public static class AuthorizationHandlerExtensions
    {
        internal static IList<PermissionCheckAttribute> GetPermissionAttributes(this AuthorizationHandlerContext context, HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            var permissionAttributes = endpoint.Metadata.Where(x => x is PermissionCheckAttribute).Cast<PermissionCheckAttribute>().ToList();
            var overridePermissionIds = permissionAttributes.Select(x => x.OverridePermissionIds).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            if (overridePermissionIds.Count == 0)
            {
                return permissionAttributes;
            }

            var needIgnoreIds = overridePermissionIds
                .SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            var result = permissionAttributes
                .Where(x => !needIgnoreIds.Contains(x.PermissionId, StringComparer.OrdinalIgnoreCase))
                .ToList();

            return result;
        }
        
        internal static ActionDescriptor GetControllerActionDescriptor(this AuthorizationHandlerContext context, HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            return endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
        }
    }
}
