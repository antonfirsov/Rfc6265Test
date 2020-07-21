using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rfc6265TestClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string domain = args.FirstOrDefault() ?? "localhost";
        
            Console.WriteLine($" --- Cookies returned from http://{domain}:5000 ---");
            await HttpClientTest(domain);
            Console.WriteLine(" --- Cookies added by manual filling of CookieContainer ---");
            CookieContainerTest(domain);
        }

        private static void CookieContainerTest(string domain)
        {
            CookieContainer container = new CookieContainer();
            Uri url = new Uri($"http://{domain}:5000");

            void AddCookie(string name, string rest = "")
            {
                try
                {
                    container.SetCookies(url, $"{name}=xxx; {rest}");
                }
                catch (CookieException ex)
                {
                    Console.WriteLine($"Could not add {name} because of exception: {ex.Message}");
                }
            } 
            
            AddCookie("a");
            AddCookie("b", $"domain={domain}");
            AddCookie("c", $"domain=.{domain}");
            AddCookie("d", $"domain={domain}; version=1");
            AddCookie("e", "version=1");
            
            int firstDot = domain.IndexOf('.');
            if (firstDot > 0)
            {
                string rootDomain = domain.Substring(firstDot + 1, domain.Length - firstDot - 1);
                AddCookie("f", $"domain={rootDomain}");
                AddCookie("g", $"domain=.{rootDomain}");
            }
            
            PrintCookies(container, domain);
        }

        private static async Task HttpClientTest(string domain)
        {
            HttpClientHandler handler = new HttpClientHandler();
            CookieContainer cookies = handler.CookieContainer;
            using HttpClient client = new HttpClient(handler);
            await client.GetAsync($"http://{domain}:5000");

            PrintCookies(cookies, domain);
        }

        private static void PrintCookies(CookieContainer cookies, string domain)
        {
            foreach (Cookie cookie in cookies.GetCookies(new Uri($"http://{domain}:5000")))
            {
                Console.WriteLine($"{cookie} | {cookie.Domain}");
            }
        }
    }
}