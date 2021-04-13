using Common.Auth.PermissionChecks.Actions;

namespace Common.Auth.PermissionChecks.Demo
{
    public class DemoPermissionCheckActionRepository : IPermissionCheckActionRegistryRepository
    {
        private static readonly PermissionCheckActionRegistry Registry = new PermissionCheckActionRegistry();

        public PermissionCheckActionRegistry GetRegistry()
        {
            if (Registry.Actions.Count == 0)
            {
                Registry.Actions.Add(PermissionCheckAction.Create(KnownActionIds.UnsureActionId, KnownPermissionIds.UnsureOp, "某个迟决定的Action"));
                Registry.Actions.Add(PermissionCheckAction.Create(KnownActionIds.UnsureActionId2, KnownPermissionIds.UnsureOp, "某个迟决定的Action2"));
            }
            return Registry;
        }

        public void SaveRegistry(PermissionCheckActionRegistry registry)
        {
        }
    }
}