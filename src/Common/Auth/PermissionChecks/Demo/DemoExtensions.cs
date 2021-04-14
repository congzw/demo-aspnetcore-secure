using Common.Auth.PermissionChecks.ControlPoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Auth.PermissionChecks.Demo
{
    public static class DemoExtensions
    {
        public static void AddPermissionCheckDemos(this IServiceCollection services)
        {
            services.AddSingleton<IPermissionCheckLogicProvider, DemoBasedLogic>();
            services.Replace(ServiceDescriptor.Singleton<IControlPointRegistryRepository, DemoControlPointRegistryRepository>());
        }
    }
}
