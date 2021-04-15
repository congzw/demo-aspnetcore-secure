using System.Collections.Generic;
using System.Linq;
using Common.Auth.PermissionChecks.ControlPoints;

namespace Common.Auth.PermissionChecks
{
    public class PermissionCheckContext
    {
        public PermissionCheckRequirement Requirement { get; set; }
        public List<string> NeedCheckPermissionIds { get; set; } = new List<string>();
        public CurrentUserContext UserContext { get; set; } = CurrentUserContext.Empty;
        public ControlPointRegistry ControlPointRegistry { get; set; }

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
        public List<string> MatchedNeedCheckPermissionIds(IEnumerable<string> permissionIds)
        {
            return permissionIds.Where(MatchPermissionId).ToList();
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

        public static PermissionCheckContext Create(ControlPointRegistry registry, CurrentUserContext userContext, PermissionCheckRequirement requirement, params string[] permissionIds)
        {
            var context = new PermissionCheckContext
            {
                ControlPointRegistry =  registry,
                Requirement = requirement,
                UserContext = userContext
            };
            context.AddCheckPermissionIds(permissionIds);
            return context;
        }
    }
}