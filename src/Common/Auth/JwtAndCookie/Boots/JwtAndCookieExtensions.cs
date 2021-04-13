using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Auth.JwtAndCookie.Demo;
using Common.Auth.PermissionChecks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Common.Auth.JwtAndCookie.Boots
{
    public static class JwtAndCookieExtensions
    {
        private static string jwtBearerScheme = JwtBearerDefaults.AuthenticationScheme; // "Bearer"
        private static string cookieScheme = CookieAuthenticationDefaults.AuthenticationScheme; //"Cookies"

        public static void AddJwtAndCookie(this IServiceCollection services)
        {
            //认证
            AddAuthenticate(services);
            //扩展Claims
            AddClaimsTransformation(services);
        }

        private static void AddAuthenticate(IServiceCollection services)
        {
            var authenticationBuilder = services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = jwtBearerScheme;
                o.DefaultChallengeScheme = jwtBearerScheme;
            });

            authenticationBuilder.AddJwtBearer(options =>
            {
                // Forward any requests to cookie scheme that not start with /api to that scheme
                options.ForwardDefaultSelector = ctx => ctx.Request.Path.StartsWithSegments("/api")
                    ? null
                    : cookieScheme;

                var jwtSetting = JwtSetting.Instance;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSetting.Issuer,
                    ValidAudience = jwtSetting.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Key))
                };

                var jwtBearerEvents = new JwtBearerEvents();
                options.Events = jwtBearerEvents;
                //测试用 => 模拟发来的Authorization: "Bearer Token"
                jwtBearerEvents.OnMessageReceived = context =>
                {
                    if (string.IsNullOrWhiteSpace(MockClientRequest.Instance.Token))
                    {
                        return Task.CompletedTask;
                    }

                    //模拟客户端发来的Bearer Token
                    var theToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                    if (theToken == null)
                    {
                        context.Request.Headers["Authorization"] = "Bearer " + MockClientRequest.Instance.Token;
                    }
                    context.Token = MockClientRequest.Instance.Token;
                    return Task.CompletedTask;
                };
            });

            authenticationBuilder.AddCookie(cookieScheme, options =>
            {
                // Forward any requests that start with /api to that scheme
                options.ForwardDefaultSelector = ctx => ctx.Request.Path.StartsWithSegments("/api")
                    ? jwtBearerScheme
                    : null;

                options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // optional
                options.LoginPath = new PathString("/DemoAccount/Login");
                options.LogoutPath = new PathString("/DemoAccount/Logout");
                options.AccessDeniedPath = new PathString("/DemoAccount/Forbidden");

            });
        }

        private static void AddClaimsTransformation(IServiceCollection services)
        {
            //DEMO: 自定义的Claims转换
            //claims transformation is run after every Authenticate call
            services.AddTransient<IClaimsTransformation, DemoClaimsTransformer>();
        }
    }
}
