using System;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Auth.PermissionChecks.Actions
{
    public static class PermissionCheckActionExtensions
    {
        public static IServiceCollection AddPermissionCheckActions(this IServiceCollection services)
        {
            services.AddTransient(sp => sp.GetPermissionCheckActionRegistry());
            return services;
        }

        public static PermissionCheckActionRegistry GetPermissionCheckActionRegistry(this IServiceProvider sp)
        {
            var repository = sp.GetService<IPermissionCheckActionRegistryRepository>();
            if (repository != null)
            {
                return repository.GetRegistry();
            }
            return PermissionCheckActionRegistry.Default;
        }
    }
    
    public interface IPermissionCheckActionRegistryRepository
    {
        PermissionCheckActionRegistry GetRegistry();

        void SaveRegistry(PermissionCheckActionRegistry registry);
    }
}