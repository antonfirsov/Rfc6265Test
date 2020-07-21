using System.Linq;
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
            Startup.Domain = args.FirstOrDefault() ?? "localhost";

            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).Build().Run();
        }
    }
    
    public class Startup
    {
        public static string Domain = "localhost";
        
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
                    void AddCookie(string name, string stuff = "") =>
                        ctx.Response.Headers.Append("Set-Cookie", $"{name}=xxx; {stuff}");

                    AddCookie("a");
                    AddCookie("b", $"domain={Domain}");
                    AddCookie("c", $"domain=.{Domain}");
                    AddCookie("d", $"domain={Domain}; version=1");
                    AddCookie("e", "version=1");

                    int firstDot = Domain.IndexOf('.');
                    if (firstDot > 0)
                    {
                        string rootDomain = Domain.Substring(firstDot + 1, Domain.Length - firstDot - 1);
                        AddCookie("f", $"domain={rootDomain}");
                        AddCookie("g", $"domain=.{rootDomain}");
                    }

                    await ctx.Response.WriteAsync("COOKIE DOMAIN TEST");
                });
            });
        }
    }
}