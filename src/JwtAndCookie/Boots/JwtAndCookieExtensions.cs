using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JwtAndCookie.Libs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace JwtAndCookie.Boots
{
    public static class JwtAndCookieExtensions
    {
        public static void AddJwtAndCookie(this IServiceCollection services)
        {
            // JwtBearerDefaults.AuthenticationScheme == "Bearer"
            // CookieAuthenticationDefaults.AuthenticationScheme == "Cookie"

            services
                .AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    
                    //o.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                    //o.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    //o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    //o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    //o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

                })
                .AddJwtBearer(options =>
                {
                    // Forward any requests that start with /api to that scheme
                    options.ForwardDefaultSelector = ctx => !ctx.Request.Path.StartsWithSegments("/api")
                        ? CookieAuthenticationDefaults.AuthenticationScheme
                        : null;
                    
                    //todo
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
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    // Forward any requests that start with /api to that scheme
                    options.ForwardDefaultSelector = ctx => ctx.Request.Path.StartsWithSegments("/api")
                        ? JwtBearerDefaults.AuthenticationScheme
                        : null;

                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // optional
                    options.LoginPath = new PathString("/DemoAccount/Login");
                    options.LogoutPath = new PathString("/DemoAccount/Logout");
                    options.AccessDeniedPath = new PathString("/DemoAccount/Forbidden");
                });
            
            //DEMO: 自定义的Claims转换
            //claims transformation is run after every Authenticate call
            services.AddTransient<IClaimsTransformation, ClaimsTransformer>();
            
            //var multiSchemePolicy = new AuthorizationPolicyBuilder(
            //        JwtBearerDefaults.AuthenticationScheme,
            //        CookieAuthenticationDefaults.AuthenticationScheme)
            //    .RequireAuthenticatedUser()
            //    .Build();

            //services.AddAuthorization(o =>
            //{
            //    o.DefaultPolicy = multiSchemePolicy;
            //});

            ////todo: more => [Authorize("MySuperAdminPolicy")]:
            //services
            //    .AddAuthorization(o =>
            //    {
            //        o.DefaultPolicy = multiSchemePolicy;
            //        o.AddPolicy(
            //            "MySuperAdminPolicy",
            //            b => b.Requirements.Add(new MySuperAdminRequirement()));
            //    });

        }
    }

    public class JwtSetting
    {
        public string Key { get; set; }
        public string Issuer { get; set; }

        //todo: read from config
        public static JwtSetting Instance = new JwtSetting()
        {
            Issuer = "TheIssuer",
            Key = "TheKey1234567890"
        };
    }
}
