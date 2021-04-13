using System.Collections.Generic;

namespace Common.Auth.PermissionChecks.Actions
{
    /// <summary>
    /// 代表系统注册过的，需要验证权限的Action
    /// </summary>
    public class PermissionCheckActionRegistry
    {
        public List<PermissionCheckAction> Actions { get; set; } = new List<PermissionCheckAction>();

        internal static PermissionCheckActionRegistry Default = new PermissionCheckActionRegistry();
    }
}
