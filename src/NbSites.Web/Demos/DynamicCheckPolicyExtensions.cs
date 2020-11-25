using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NbSites.Web.Demos.Permissions;

namespace NbSites.Web.Demos
{
    public static class DynamicCheckPolicyExtensions
    {
        public static void AddDynamicCheckPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddTransient<IAuthorizationHandler, DynamicCheckHandlerDefault>();
            services.AddTransient<DynamicCheckRulePool>();
            services.AddSingleton<IDynamicCheckRuleRepository, DynamicCheckRuleRepository>();
            services.AddSingleton<IDynamicCheckActionRepository, DynamicCheckActionRepository>();
            services.AddTransient<ICurrentUserContextProvider, CurrentUserContextProvider>();
            services.AddScoped(sp => sp.GetRequiredService<ICurrentUserContextProvider>().GetDynamicCheckContext());
            services.AddScoped<CurrentUserContext>(sp => sp.GetRequiredService<ICurrentUserContext>() as CurrentUserContext);

            //An alternative approach is using the options pattern: bind the options section and add it to the dependency injection service container.
            services.Configure<DynamicCheckOptions>(configuration.GetSection(DynamicCheckOptions.SectionName));
            services.AddTransient(sp => sp.GetService<IOptionsSnapshot<DynamicCheckOptions>>().Value); //ok => use "IOptionsSnapshot<>" instead of "IOptions<>" will auto load after changed
            services.AddTransient<IAuthorizationHandler, DynamicCheckNakedHandler>();

            services.AddScoped<IPermissionCheckService, PermissionCheckService>();
            services.AddScoped<IPermissionCheckProvider, MockPermissionCheckProvider>();

            services.AddScoped<IAuthorizationHandler, DynamicCheckPermissionHandler>();
            

            ////An alternative way for MVC controllers and Razor Pages
            //services.AddControllers(config =>
            //{
            //    var policy = new AuthorizationPolicyBuilder()
            //        .AddRequirements(new DynamicCheckFeatureRequirement())
            //        .Build();
            //    config.Filters.Add(new AuthorizeFilter(policy));
            //});

            services.AddAuthorization(options =>
            {
                var dynamicCheckPolicy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new DynamicCheckRequirement())
                    .Build();

                //var defaultPolicy = new AuthorizationPolicyBuilder()
                //    .Combine(dynamicCheckPolicy)
                //    .Build();

                options.DefaultPolicy = dynamicCheckPolicy;
                options.FallbackPolicy = dynamicCheckPolicy;


                //options.AddPolicy("DynamicCheckOp", policy =>
                //{
                //    policy.Requirements.Add(new DynamicCheckFeatureRequirement());
                //});

                //options.DefaultPolicy = ...
                //options.FallbackPolicy = ...

                ////除非显示指定，否则全局都被安全控制（建议采用）
                ////备用策略：有类似声明则不应用：[AllowAnonymous],[Authorize(PolicyName="MyPolicy")]
                //options.FallbackPolicy = new AuthorizationPolicyBuilder()
                //    .AddRequirements(new DynamicCheckFeatureRequirement())
                //    .Build();


                ////除非显示指定，否则全局都被安全控制（建议采用）
                ////备用策略：有类似声明则不应用：[AllowAnonymous],[Authorize(PolicyName="MyPolicy")]
                //options.FallbackPolicy = new AuthorizationPolicyBuilder()
                //    .RequireAuthenticatedUser()
                //    .Build();
            });
        }
    }
}