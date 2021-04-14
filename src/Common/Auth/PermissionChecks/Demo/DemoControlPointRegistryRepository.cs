using Common.Auth.PermissionChecks.ControlPoints;

namespace Common.Auth.PermissionChecks.Demo
{
    /// <summary>
    /// 演示用的内存仓储，重启失效
    /// </summary>
    public class DemoControlPointRegistryRepository : IControlPointRegistryRepository
    {
        private static ControlPointRegistry _registry = new ControlPointRegistry();

        public DemoControlPointRegistryRepository()
        {
            //增加演示用的数据
            DemoConst.Init(_registry);
        }
        
        public ControlPointRegistry GetControlPointRegistry()
        {
            return _registry;
        }

        public void SaveControlPointRegistry(ControlPointRegistry registry)
        {
            _registry = registry;
        }
    }
}
