using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace NbSites.Web.Demos
{
    public static class DynamicCheckPolicyExtensions
    {
        public static void AddDynamicCheckPolicy(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddTransient<IAuthorizationHandler, DynamicCheckFeatureHandler>();
            services.AddTransient<DynamicCheckFeatureService>();
            services.AddSingleton<ICheckFeatureRuleRepository, CheckFeatureRuleRepository>();

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
                    .AddRequirements(new DynamicCheckFeatureRequirement())
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