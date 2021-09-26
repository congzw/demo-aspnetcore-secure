using AspNet.Security.OAuth.Gitee;
using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OAuthSample
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
            //todo: DefaultScheme vs DefaultChallengeScheme? 
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizePage("/Contact");
            });
            
            AddTheGitee(services);
            //AddTheGithub(services);
        }

        private void AddTheGithub(IServiceCollection services)
        {
            var defaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = defaultScheme;
                    options.DefaultChallengeScheme = GitHubAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(defaultScheme, options =>
                {
                })
                .AddGitHub(options =>
                {
                    options.ClientId = Configuration["GitHub:ClientId"];
                    options.ClientSecret = Configuration["GitHub:ClientSecret"];
                    options.CallbackPath = "/Account/GithubCallback";
                    //检查配置地址是否一致！
                    //error=redirect_uri_mismatch&error_description=The+redirect_uri+MUST+match+the+registered+callback+URL+for+this+application.&
                    //options.CallbackPath = new PathString("/Account/GitHubCallback");
                });
        }

        private void AddTheGitee(IServiceCollection services)
        {
            var defaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = defaultScheme;
                    options.DefaultChallengeScheme = GiteeAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(defaultScheme, options =>
                {
                    //options.LoginPath = new PathString("/Account/Login");
                    //options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                })
                .AddGitee(options =>
                {
                    options.ClientId = Configuration["Gitee:ClientId"];
                    options.ClientSecret = Configuration["Gitee:ClientSecret"];
                    //fix: 无效的登录回调地址
                    //"/Account/GiteeCallback" => AddGitee的实现，并不需要真实存在，但必须配置
                    options.CallbackPath = new PathString("/Account/GiteeCallback");
                });
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

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
