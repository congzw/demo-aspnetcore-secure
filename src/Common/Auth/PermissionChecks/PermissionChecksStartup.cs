using System.Linq;
using Common.Auth.PermissionChecks.AuthorizationHandlers.Demo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Auth.PermissionChecks
{
    public static class PermissionChecksStartup
    {
        public static void AddPermissionChecks(this IServiceCollection services)
        {
            var httpContextAccessorDescriptor = services.LastOrDefault(d => d.ServiceType == typeof(IHttpContextAccessor));
            if (httpContextAccessorDescriptor == null)
            {
                services.AddHttpContextAccessor();
            }

            services.AddAuthorization(options =>
            {
                //DefaultPolicy:    针对空的[Authorize]（attribute without any PolicyName.）的Endpoints
                //FallbackPolicy:   没有任何[Authorize]或[AllowAnonymous]的Endpoints

                //让DefaultPolicy保持默认。这样自定义PermissionCheckRequirement的作用范围，就是FallbackPolicy
                var dynamicCheckPolicy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionCheckRequirement())
                    .Build();

                options.FallbackPolicy = dynamicCheckPolicy;
            });

            services.AddTransient<IAuthorizationHandler, DemoBasedHandler>();
            //services.AddTransient<IAuthorizationHandler, ConfigBasedHandler>();
            
            //services.AddAuthorization(options =>
            //{
            //    ////DefaultPolicy:    针对空的[Authorize]（attribute without any PolicyName.）的Endpoints
            //    ////默认的授权策略，等同于默认
            //    //var defaultPolicy = new AuthorizationPolicyBuilder()
            //    // .RequireAuthenticatedUser()
            //    // .Build();
            //    //options.DefaultPolicy = defaultPolicy;

            //    //FallbackPolicy:   没有任何[Authorize]或[AllowAnonymous]的Endpoints
            //    var dynamicCheckPolicy = new AuthorizationPolicyBuilder("Bearer", "Cookies")
            //        .AddRequirements(new PermissionCheckRequirement())
            //        .Build();

            //    options.FallbackPolicy = dynamicCheckPolicy;

            //    //demo based
            //    SetupDemoBased(services);

            //    ////config based
            //    //SetupConfigBased(services);
            //    //services.AddTransient<IAuthorizationHandler, RoleBasedPermissionRuleHandler>();
            //    //services.AddTransient<IAuthorizationHandler, ResourceBasedPermissionCheckHandler>();
            //});
        }

        public static IApplicationBuilder UsePermissionChecks(this IApplicationBuilder appBuilder)
        {
            //using (var scope = appBuilder.ApplicationServices.CreateScope())
            //{
            //    var permissionCheckDebugHelper = scope.ServiceProvider.GetRequiredService<IPermissionCheckDebugHelper>();
            //    var dynamicCheckOptions = scope.ServiceProvider.GetRequiredService<PermissionCheckOptions>();
            //    var debugHelperEnabled = dynamicCheckOptions.DebugHelperEnabled;
            //    permissionCheckDebugHelper.Enabled = () => debugHelperEnabled;

            //    var poolService = scope.ServiceProvider.GetRequiredService<IPermissionRuleActionPoolService>();
            //    var pool = scope.ServiceProvider.GetRequiredService<IPermissionRuleActionPool>();
            //    poolService.RefreshPool(pool);
            //}
            return appBuilder;
        }
    }
}
