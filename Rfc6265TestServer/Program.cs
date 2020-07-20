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