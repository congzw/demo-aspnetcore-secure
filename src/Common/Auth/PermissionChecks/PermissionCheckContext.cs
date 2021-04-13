using System.Collections.Generic;
using System.Linq;

namespace Common.Auth.PermissionChecks
{
    public class PermissionCheckContext
    {
        public PermissionCheckRequirement Requirement { get; set; }
        public List<string> NeedCheckPermissionIds { get; set; } = new List<string>();
        public CurrentUserContext UserContext { get; set; } = CurrentUserContext.Empty;

        public bool MatchPermissionId(string permissionId)
        {
            if (NeedCheckPermissionIds == null || NeedCheckPermissionIds.Count == 0)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(permissionId))
            {
                return false;
            }

            return NeedCheckPermissionIds.MyContains(permissionId);
        }

        public PermissionCheckContext AddCheckPermissionIds(params string[] permissionIds)
        {
            if (permissionIds == null || permissionIds.Length == 0)
            {
                return this;
            }

            foreach (var permissionId in permissionIds)
            {
                if (!NeedCheckPermissionIds.MyContains(permissionId))
                {
                    if (!string.IsNullOrWhiteSpace(permissionId))
                    {
                        var splitIds = permissionId.SplitToValues().Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                        foreach (var splitId in splitIds)
                        {
                            NeedCheckPermissionIds.Add(splitId);
                        }
                    }
                }
            }
            return this;
        }

        public static PermissionCheckContext Create(CurrentUserContext userContext, PermissionCheckRequirement requirement, params string[] permissionIds)
        {
            var context = new PermissionCheckContext
            {
                Requirement = requirement,
                UserContext = userContext
            };
            context.AddCheckPermissionIds(permissionIds);
            return context;
        }
    }
}