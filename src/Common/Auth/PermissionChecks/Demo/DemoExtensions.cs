using Microsoft.Extensions.DependencyInjection;

namespace Common.Auth.PermissionChecks.Demo
{
    public static class DemoExtensions
    {
        public static void AddPermissionCheckDemos(this IServiceCollection services)
        {
            services.AddSingleton<IPermissionCheckLogicProvider, DemoCheckLogic>();
        }
    }
}
