using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NbSites.Web.PermissionChecks.ConfigBased;
using NbSites.Web.PermissionChecks.RoleBased;

namespace NbSites.Web.PermissionChecks
{
    public static class PermissionChecksStartup
    {
        public static void AddPermissionChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorization(options =>
            {
                var dynamicCheckPolicy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionCheckRequirement())
                    .Build();

                options.DefaultPolicy = dynamicCheckPolicy;
                options.FallbackPolicy = dynamicCheckPolicy;
            });
            services.AddHttpContextAccessor();

            services.AddScoped<ICurrentUserContextProvider, CurrentUserContextProvider>();
            services.AddScoped(sp => sp.GetRequiredService<ICurrentUserContextProvider>().GetCurrentUserContext());
            services.AddSingleton<IDynamicCheckActionRepository, DynamicCheckActionRepository>(); //todo: replace with real scope impl
            services.Configure<DynamicCheckOptions>(configuration.GetSection(DynamicCheckOptions.SectionName));
            services.AddTransient(sp => sp.GetService<IOptionsSnapshot<DynamicCheckOptions>>().Value); //ok => use "IOptionsSnapshot<>" instead of "IOptions<>" will auto load after changed

            //config based
            services.AddTransient<IAuthorizationHandler, ConfigBasedHandler>();

            //role based
            services.AddSingleton<IPermissionRuleActionPool, PermissionRuleActionPool>();
            services.AddScoped<IPermissionRuleActionPoolInitService, PermissionRuleActionPoolInitService>();
            services.AddScoped<IPermissionRuleActionProvider, PermissionRuleActionProvider>();
            services.AddScoped<IAuthorizationHandler, RoleBasedPermissionRuleHandler>();
            services.AddTransient<IRoleBasedPermissionRuleLogic, RoleBasedPermissionRuleLogic>();
            services.AddSingleton<IRoleBasedPermissionRuleRepository, RoleBasedPermissionRuleRepository>(); //todo: replace with real scope impl
        }

        public static IApplicationBuilder UsePermissionChecks(this IApplicationBuilder appBuilder)
        {
            return appBuilder;
        }
    }
}
