using System.Collections.Generic;
using Common.Auth.PermissionChecks.Actions;

namespace Common.Auth.PermissionChecks.Demo
{
    public class DemoPermissionCheckActionRepository : IPermissionCheckActionRepository
    {
        public DemoPermissionCheckActionRepository()
        {
            //Actions.Add(DynamicCheckAction.Create(KnownActionIds.UnsureActionId, KnownPermissionIds.UnsureOp, "某个迟决定的Action"));
            //Actions.Add(DynamicCheckAction.Create(KnownActionIds.UnsureActionId2, KnownPermissionIds.UnsureOp, "某个迟决定的Action2"));
        }

        public IList<PermissionCheckAction> Actions { get; set; } = new List<PermissionCheckAction>();

        public IList<PermissionCheckAction> GetActions()
        {
            //todo: read from real data source
            return Actions;
        }

        public void SaveAll(IList<PermissionCheckAction> actions)
        {
        }
    }
}