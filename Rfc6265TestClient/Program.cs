using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rfc6265TestClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine(" --- Cookies returned from http://localhost:5000 ---");
            await HttpClientTest();
            Console.WriteLine(" --- Cookies added by manual filling of CookieContainer ---");
            CookieContainerTest();
        }

        private static void CookieContainerTest()
        {
            CookieContainer container = new CookieContainer();
            Uri url = new Uri("http://localhost:5000");

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
            AddCookie("b", "domain=localhost");
            AddCookie("c", "domain=.localhost");
            AddCookie("d", "domain=localhost; version=1");
            AddCookie("e", "version=1");
            
            PrintCookies(container);
        }

        private static async Task HttpClientTest()
        {
            HttpClientHandler handler = new HttpClientHandler();
            CookieContainer cookies = handler.CookieContainer;
            using HttpClient client = new HttpClient(handler);
            await client.GetAsync("http://localhost:5000");

            PrintCookies(cookies);
        }

        private static void PrintCookies(CookieContainer cookies)
        {
            foreach (Cookie cookie in cookies.GetCookies(new Uri("http://localhost:5000")))
            {
                Console.WriteLine($"{cookie} | {cookie.Domain}");
            }
        }
    }
}