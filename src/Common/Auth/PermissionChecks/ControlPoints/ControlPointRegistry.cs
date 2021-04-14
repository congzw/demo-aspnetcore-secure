using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Common.Auth.PermissionChecks.AuthorizationHandlers.RoleBased;

namespace Common.Auth.PermissionChecks.ControlPoints
{
    public class ControlPointRegistry
    {
        public IDictionary<string, Permission> Permissions { get; set; } = new ConcurrentDictionary<string, Permission>(StringComparer.OrdinalIgnoreCase);
        public IDictionary<string, EndPoint> EndPoints { get; set; } = new ConcurrentDictionary<string, EndPoint>(StringComparer.OrdinalIgnoreCase);
        public IDictionary<string, EndPointPermission> EndPointPermissions { get; set; } = new ConcurrentDictionary<string, EndPointPermission>(StringComparer.OrdinalIgnoreCase);
        public IDictionary<string, PermissionRule> PermissionRules { get; set; } = new ConcurrentDictionary<string, PermissionRule>(StringComparer.OrdinalIgnoreCase);
        
        public ControlPointRegistry SetPermission(Permission permission)
        {
            Permissions[permission.Id] = permission;
            return this;
        }
        public ControlPointRegistry SetEndPoint(EndPoint endPoint)
        {
            EndPoints[endPoint.Id] = endPoint;
            return this;
        }
        public ControlPointRegistry SetEndPointPermission(EndPointPermission relation)
        {
            EndPointPermissions[relation.GetLocateKey()] = relation;
            return this;
        }
        public ControlPointRegistry RemoveEndPointPermission(EndPointPermission relation)
        {
            EndPointPermissions.Remove(relation.GetLocateKey());
            return this;
        }

        public ControlPointRegistry SetPermissionRule(PermissionRule rule)
        {
            PermissionRules[rule.PermissionId] = rule;
            return this;
        }
        public ControlPointRegistry RemovePermissionRule(PermissionRule rule)
        {
            PermissionRules.Remove(rule.PermissionId);
            return this;
        }

        public List<EndPointPermission> GetEndPointPermissions(string endPointId)
        {
            var pointPermissions = EndPointPermissions
                .Values
                .Where(x => x.EndPointId.MyEquals(endPointId)).ToList();

            var needIgnoreIds = EndPointPermissions.Values
                .Where(x => !string.IsNullOrWhiteSpace(x.OverridePermissionIds))
                .SelectMany(x => x.OverridePermissionIds.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            var result = pointPermissions
                .Where(x => !needIgnoreIds.Contains(x.PermissionId, StringComparer.OrdinalIgnoreCase))
                .ToList();
            
            return result;
        }
    }

    public class Permission
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public static Permission Create(string id, string name)
        {
            return new Permission() {Id = id, Name = id};
        }
    }

    public class EndPoint
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public static EndPoint Create(string id, string name)
        {
            return new EndPoint() { Id = id, Name = id };
        }
    }

    public class EndPointPermission
    {
        public string EndPointId { get; set; }
        public string PermissionId { get; set; }
        public string OverridePermissionIds { get; set; }
        public string GetLocateKey()
        {
            return $"{EndPointId}-{PermissionId}";
        }

        public static EndPointPermission Create(string endPointId, string permissionId, string overridePermissionIds = null)
        {
            return new EndPointPermission() {EndPointId = endPointId, PermissionId = permissionId, OverridePermissionIds = overridePermissionIds};
        }
    }

    public class PermissionRule
    {
        public string PermissionId { get; set; }
        public string Expression { get; set; }

        public static PermissionRule Create(RoleBasedRuleExpression expression)
        {
            return new PermissionRule(){PermissionId = expression.PermissionId, Expression = expression.ToExpression()};
        }
    }
}
