using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Rfc6265TestServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).Build().Run();
        }
    }
    
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async ctx =>
                {
                    if (!ctx.Request.Cookies.ContainsKey("LoginInvoked"))
                    {
                        ctx.Response.Redirect("/login/user");
                    }
                    else
                    {
                        if (ctx.Request.Cookies.ContainsKey("LoggedIn"))
                        {
                            await ctx.Response.WriteAsync($"WORKS");
                        }
                        else
                        {
                            await ctx.Response.WriteAsync($"COOKIE MISSING LoggedIn");
                        }
                    }
                });

                endpoints.MapGet("/login/user", async ctx =>
                {
                    ctx.Response.Cookies.Append("LoginInvoked", "true", new CookieOptions() { Path = "/" });

                    ctx.Response.Cookies.Append("LoggingIn", "true", new CookieOptions() { Path = "/return" });

                    ctx.Response.Redirect("/return");
                    await ctx.Response.CompleteAsync();
                });


                endpoints.MapGet("/return", async ctx =>
                {
                    if (!ctx.Request.Cookies.ContainsKey("LoggingIn"))
                    {
                        await ctx.Response.WriteAsync($"COOKIE MISSING LoggingIn");
                        return;
                    }

                    ctx.Response.Cookies.Append("LoggedIn", "true", new CookieOptions() { Path = "/" });

                    ctx.Response.Redirect("/");
                });
                
                // Basic redirect test case:

                endpoints.MapGet("/basictest", async ctx =>
                {
                    if (!ctx.Request.Cookies.ContainsKey("Basic-LoginInvoked"))
                    {
                        ctx.Response.Redirect("/basictest/login");
                    }
                    else
                    {
                        if (ctx.Request.Cookies.ContainsKey("Basic-LoggedIn"))
                        {
                            await ctx.Response.WriteAsync($"WORKS");
                        }
                        else
                        {
                            await ctx.Response.WriteAsync($"COOKIE MISSING Basic-LoggedIn");
                        }
                    }
                });
                
                endpoints.MapGet("/basictest/login", async ctx =>
                {
                    ctx.Response.Cookies.Append("Basic-LoginInvoked", "true", new CookieOptions() { Path = "/basictest"} );
                    ctx.Response.Cookies.Append("Basic-LoggedIn", "true", new CookieOptions() { Path = "/basictest/login"});
                    ctx.Response.Redirect("/basictest");
                    await ctx.Response.CompleteAsync();
                });

                endpoints.MapGet("/domaintest", async ctx =>
                {
                    void AddCookie(string name, string stuff = "") => ctx.Response.Headers.Append("Set-Cookie", $"{name}=xxx; {stuff}");
                    
                    AddCookie("a");
                    AddCookie("b", "domain=localhost");
                    AddCookie("c", "domain=.localhost");
                    AddCookie("d", "domain=localhost; version=1");
                    AddCookie("e", "version=1");

                    await ctx.Response.WriteAsync("COOKIE DOMAIN TEST");
                });
            });
        }
    }
}