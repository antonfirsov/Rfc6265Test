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
            });
        }
    }
}