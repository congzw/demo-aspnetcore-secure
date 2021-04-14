namespace Common.Auth.PermissionChecks.ControlPoints
{
    public interface IControlPointRegistryRepository
    {
        /// <summary>
        /// 加载
        /// </summary>
        /// <returns></returns>
        ControlPointRegistry GetControlPointRegistry();

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="registry"></param>
        void SaveControlPointRegistry(ControlPointRegistry registry);
    }

    /// <summary>
    /// 默认实现: 单例的内存存储
    /// 可以用新的实现替换之，如: 数据库等存储, 配置文件存储
    /// </summary>
    public class ControlPointRegistryRepository : IControlPointRegistryRepository
    {
        private static ControlPointRegistry _registry  = new ControlPointRegistry();

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
