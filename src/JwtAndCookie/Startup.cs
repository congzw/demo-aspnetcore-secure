using Common.Auth;
using Common.Auth.JwtAndCookie.Boots;
using Common.Auth.PermissionChecks;
using Common.Auth.PermissionChecks.AuthorizationHandlers;
using Common.Auth.PermissionChecks.Demo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JwtAndCookie
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            PermissionCheckDebugHelper.Instance.Enabled = () => true;
            services.AddCurrentUserContext();
            services.AddJwtAndCookie();
            services.AddPermissionChecks();
            services.AddPermissionCheckDemos();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute
                (
                    name: "Default",
                    pattern: "{controller}/{action}",
                    defaults: new { controller = "Demo", action = "Index" }
                );
            });
        }
    }
}
