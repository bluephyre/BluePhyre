using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using BluePhyre.Core.Interfaces.Repositories;
using BluePhyre.Infrastructure.Apis.DnSimple;
using BluePhyre.Infrastructure.Apis.Linode;
using BluePhyre.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace BluePhyre.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }).AddCookie()
                .AddGoogle("google", options =>
                {
                    options.ClientId = Configuration["Google:ClientId"];
                    options.ClientSecret = Configuration["Google:ClientSecret"];

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");

                    options.CallbackPath = new PathString("/signin-google");

                    options.ClaimsIssuer = "Google";
                    options.SaveTokens = true;

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = context =>
                        {
                            var user = context.Principal.Claims.FirstOrDefault(c =>
                                c.Type == ClaimTypes.NameIdentifier);

                            var repository =
                                context.HttpContext.RequestServices.GetRequiredService<IClientRepository>();

                            if (repository.IsUserSuperAdmin(user?.Value))
                            {
                                var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Role, "superadmin")
                                };

                                context.Principal.AddIdentity(new ClaimsIdentity(claims));

                            }

                            return Task.CompletedTask;
                        }
                    };
                })
                .AddFacebook("facebook", options =>
                {
                    options.ClientId = Configuration["Facebook:ClientId"];
                    options.ClientSecret = Configuration["Facebook:ClientSecret"];

                    options.Scope.Clear();
                    options.Scope.Add("public_profile");
                    options.Scope.Add("email");

                    options.CallbackPath = new PathString("/signin-facebook");

                    options.ClaimsIssuer = "Facebook";
                    options.SaveTokens = true;

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = context =>
                        {
                            var user = context.Principal.Claims.FirstOrDefault(c =>
                                c.Type == ClaimTypes.NameIdentifier);

                            var repository =
                                context.HttpContext.RequestServices.GetRequiredService<IClientRepository>();

                            if (repository.IsUserSuperAdmin(user?.Value))
                            {
                                var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Role, "superadmin")
                                };

                                context.Principal.AddIdentity(new ClaimsIdentity(claims));

                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddMvc();
            services.AddSingleton<IClientRepository, ClientRepository>();
            services.AddTransient<DnSimpleApi>(sp => new DnSimpleApi(Configuration["DnSimple:Username"], Configuration["DnSimple:Password"]));
            services.AddTransient<LinodeApi>(sp => new LinodeApi(Configuration["Linode:DomainToken"]));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("Home/Error");
            }

            app.UseStatusCodePages();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "admin",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
