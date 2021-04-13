using Common.Auth;
using Common.Auth.JwtAndCookie;
using Common.Auth.JwtAndCookie.Boots;
using Common.Auth.PermissionChecks;
using Common.Auth.PermissionChecks.AuthorizationHandlers;
using Common.Auth.PermissionChecks.Demo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JwtAndCookie
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IJwtTokenService, JwtTokenService>();
            services.AddTransient<CurrentUserContext>(sp =>
                sp.GetService<IHttpContextAccessor>()?.HttpContext?.GetCurrentUserContext() ?? CurrentUserContext.Empty);

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
