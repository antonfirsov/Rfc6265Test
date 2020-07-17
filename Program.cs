using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Rfc6265Test
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
                        ctx.Response.Redirect("/login");
                    }
                    else
                    {
                        if (ctx.Request.Cookies.ContainsKey("LoggedIn"))
                        {
                            await ctx.Response.WriteAsync($"WORKS");
                        }
                        else
                        {
                            await ctx.Response.WriteAsync($"COOKIE MISSING");
                        }
                    }
                });
                
                endpoints.MapGet("/login", async ctx =>
                {
                    ctx.Response.Cookies.Append("LoginInvoked", "true", new CookieOptions() { Path = "/"} );
                    
                    // Case 1: more specific path defined in header - COOKIE MISSING
                    ctx.Response.Cookies.Append("LoggedIn", "true", new CookieOptions() { Path = "/login"});
                    
                    // Case 2: Path is less specific - WORKS
                    // ctx.Response.Cookies.Append("LoggedIn", "true", new CookieOptions() { Path = "/"});
                    
                    // Case 3: Path is not defined (triggers default-path calculation on client) - WORKS
                    // ctx.Response.Cookies.Append("LoggedIn", "true");
                    
                    ctx.Response.Redirect("/");
                    await ctx.Response.CompleteAsync();
                });
            });
        }
    }
}