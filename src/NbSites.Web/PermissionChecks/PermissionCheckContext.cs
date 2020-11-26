using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace NbSites.Web.PermissionChecks
{
    public class PermissionCheckContext
    {
        public ActionDescriptor ActionDescriptor { get; set; }
        public HttpContext HttpContext { get; set; }
        public ICurrentUserContext CurrentUserContext { get; set; }
        public List<string> CheckPermissionIds { get; set; } = new List<string>();
        
        public bool MatchPermissionId(string permissionId)
        {
            if (CheckPermissionIds == null || CheckPermissionIds.Count == 0)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(permissionId))
            {
                return false;
            }

            return CheckPermissionIds.MyContains(permissionId);
        }

        public PermissionCheckContext AddCheckPermissionIds(params string[] permissionIds)
        {
            if (permissionIds == null || permissionIds.Length == 0)
            {
                return this;
            }

            foreach (var permissionId in permissionIds)
            {
                if (!CheckPermissionIds.MyContains(permissionId))
                {
                    if (!string.IsNullOrWhiteSpace(permissionId))
                    {
                        CheckPermissionIds.Add(permissionId);
                    }
                }
            }
            return this;
        }
        
        public PermissionCheckContext AddCheckPermissionIdsValue(string permissionIdsValue)
        {
            if (string.IsNullOrWhiteSpace(permissionIdsValue))
            {
                return this;
            }

            var permissionIds = permissionIdsValue.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return AddCheckPermissionIds(permissionIds);
        }
    }
}
