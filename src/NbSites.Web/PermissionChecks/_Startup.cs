using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NbSites.Web.Demos;
using NbSites.Web.Demos.CheckLogics;
using NbSites.Web.Demos.RuleActions;
using NbSites.Web.PermissionChecks.ConfigBased;
using NbSites.Web.PermissionChecks.ResourceBased;
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

                //options.AddPolicy("ResourceBasedPolicy", policy =>
                //    policy.Requirements.Add(new OperationAuthorizationRequirement()));
            });
            
            var httpContextAccessorDescriptor = services.LastOrDefault(d => d.ServiceType == typeof(IHttpContextAccessor));
            if (httpContextAccessorDescriptor == null)
            {
                services.AddHttpContextAccessor();
            }

            services.AddScoped<ICurrentUserContextService, CurrentUserContextService>();
            services.AddScoped(sp => sp.GetRequiredService<ICurrentUserContextService>().GetCurrentUserContext());
            services.AddSingleton<IDynamicCheckActionRepository, DynamicCheckActionRepository>(); //todo: replace with real scope impl
            services.Configure<PermissionCheckOptions>(configuration.GetSection(PermissionCheckOptions.SectionName));
            services.AddTransient(sp => sp.GetService<IOptionsSnapshot<PermissionCheckOptions>>().Value); //ok => use "IOptionsSnapshot<>" instead of "IOptions<>" will auto load after changed

            services.AddSingleton<IPermissionCheckDebugHelper, PermissionCheckDebugHelper>();

            //super power
            services.AddScoped<SuperPowerCheck>();
            //services.AddScoped<ISuperPowerProvider, SomeSuperPowerProvider>();
            
            //config based
            services.AddTransient<IAuthorizationHandler, ConfigBasedHandler>();

            //role based
            services.AddSingleton<IPermissionRuleActionPool, PermissionRuleActionPool>();
            services.AddScoped<IPermissionRuleActionPoolService, PermissionRuleActionPoolService>();
            services.AddScoped<IPermissionRuleActionProvider, PermissionRuleActionProvider>();
            services.AddScoped<IAuthorizationHandler, RoleBasedPermissionRuleHandler>();
            services.AddScoped<IAuthorizationHandler, ResourceBasedPermissionCheckHandler>();
            services.AddTransient<IRoleBasedCheckLogic, RoleBasedCheckLogic>();
            services.AddSingleton<IRoleBasedPermissionRuleRepository, RoleBasedPermissionRuleRepository>(); //todo: replace with real scope impl

            //claims based
            services.AddScoped<IPermissionCheckLogicProvider, DemoOpCheckLogicProvider>();
            services.AddScoped<IPermissionCheckLogicProvider, DemoOp2CheckLogicProvider>();

            //resource based
            services.AddScoped<IResourceBasedCheckLogicProvider, DemoOp3CheckLogicProvider>();

            //demo
            services.AddScoped<IPermissionRuleActionProvider, DemoPermissionRuleActionProvider>();
        }

        public static IApplicationBuilder UsePermissionChecks(this IApplicationBuilder appBuilder)
        {
            using (var scope = appBuilder.ApplicationServices.CreateScope())
            {
                var permissionCheckDebugHelper = scope.ServiceProvider.GetRequiredService<IPermissionCheckDebugHelper>();
                var dynamicCheckOptions = scope.ServiceProvider.GetRequiredService<PermissionCheckOptions>();
                var debugHelperEnabled = dynamicCheckOptions.DebugHelperEnabled;
                permissionCheckDebugHelper.Enabled = () => debugHelperEnabled;

                var poolService = scope.ServiceProvider.GetRequiredService<IPermissionRuleActionPoolService>();
                var pool = scope.ServiceProvider.GetRequiredService<IPermissionRuleActionPool>();
                poolService.RefreshPool(pool);
            }
            return appBuilder;
        }
    }
}
