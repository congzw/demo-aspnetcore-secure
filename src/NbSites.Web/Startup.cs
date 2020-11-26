using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NbSites.Web.PermissionChecks;

namespace NbSites.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            //services.AddRazorPages();

            //CookieAuthenticationDefaults.AuthenticationScheme = "Cookies"
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.LoginPath = new PathString("/Account/Login");
                        options.LogoutPath = new PathString("/Account/Logout");
                        options.AccessDeniedPath = new PathString("/Account/Forbidden");
                    });

            //services.AddAuthorization(options =>
            //{
            //    //添加声明策略，主动使用。
            //    options.AddPolicy("RequireManageRole", policy => policy.RequireRole("Administrator"));

            //    //var dynamicCheckPolicy = new AuthorizationPolicyBuilder()
            //    //    .AddRequirements(new DynamicCheckFeatureRequirement())
            //    //    .Build();

            //    //var defaultPolicy = new AuthorizationPolicyBuilder()
            //    //    .AddRequirements(new DynamicCheckFeatureRequirement())
            //    //    .RequireAuthenticatedUser()
            //    //    .Build();

            //    //options.DefaultPolicy = defaultPolicy;
            //    //options.FallbackPolicy = dynamicCheckPolicy;


            //    ////除非显示指定，否则全局都被安全控制（建议采用）
            //    ////备用策略：有类似声明则不应用：[AllowAnonymous],[Authorize(PolicyName="MyPolicy")]
            //    //options.FallbackPolicy = new AuthorizationPolicyBuilder()
            //    //    .RequireAuthenticatedUser()
            //    //    .Build();
            //});

            services.AddPermissionChecks(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UsePermissionChecks();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapDefaultControllerRoute();
                endpoints.MapControllerRoute(name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
