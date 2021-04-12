using Common.Auth;
using Common.Auth.JwtAndCookie;
using Common.Auth.JwtAndCookie.Boots;
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

            services.AddJwtAndCookie();

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ICurrentUserContextService, CurrentUserContextService>();
            services.AddTransient(sp => sp.GetService<ICurrentUserContextService>().GetCurrentUserContext());
            services.AddTransient<IJwtTokenService, JwtTokenService>();
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
