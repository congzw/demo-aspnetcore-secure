using System;
using System.Collections.Generic;
using System.Linq;
using Common.Auth.PermissionChecks.AuthorizationHandlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Auth.PermissionChecks.ControlPoints
{
    public interface IControlPointService
    {
        List<string> GetCurrentEndPointPermissionIds(HttpContext httpContext, string currentEndPointId);
    }

    public class ControlPointService : IControlPointService
    {
        public List<string> GetCurrentEndPointPermissionIds(HttpContext httpContext, string currentEndPointId)
        {
            var permissionIds = new List<string>();
            if (string.IsNullOrWhiteSpace(currentEndPointId))
            {
                currentEndPointId = httpContext.GetCurrentActionId();
            }
            if (currentEndPointId == null)
            {
                return permissionIds;
            }

            //从Attribute计算PermissionId
            var attrPermissionIds = httpContext.GetPermissionIdsFromAttribute();

            //从Registry计算PermissionId
            var controlPointRegistry = httpContext.RequestServices.GetService<ControlPointRegistry>();
            var regPermissionIds = controlPointRegistry.GetEndPointPermissions(currentEndPointId).Select(x => x.PermissionId).ToList();

            //合并PermissionId
            permissionIds.AddRange(attrPermissionIds);
            permissionIds.AddRange(regPermissionIds);
            permissionIds = permissionIds.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            return permissionIds;
        }
    }
}
