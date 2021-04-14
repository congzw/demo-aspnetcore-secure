using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common.Auth.PermissionChecks.ControlPoints
{
    public class ControlPointRegistry
    {
        public IDictionary<string, Permission> Permissions { get; set; } = new ConcurrentDictionary<string, Permission>();
        public IDictionary<string, EndPoint> EndPoints { get; set; } = new ConcurrentDictionary<string, EndPoint>();
        public IDictionary<string, EndPointPermission> EndPointPermissions { get; set; } = new ConcurrentDictionary<string, EndPointPermission>();
    }

    public class Permission
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class EndPoint
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class EndPointPermission
    {
        public string EndPointId { get; set; }
        public string PermissionId { get; set; }
        public string GetLocateKey()
        {
            return $"{EndPointId}-{PermissionId}";
        }
    }
}
