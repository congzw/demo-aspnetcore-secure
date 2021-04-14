using Common.Auth.PermissionChecks.Actions;

namespace Common.Auth.PermissionChecks.Demo
{
    public class DemoPermissionCheckActionRepository : IPermissionCheckActionRegistryRepository
    {
        private static readonly PermissionCheckActionRegistry Registry = new PermissionCheckActionRegistry();

        public PermissionCheckActionRegistry GetRegistry()
        {
            return Registry;
        }

        public void SaveRegistry(PermissionCheckActionRegistry registry)
        {
        }
    }
}