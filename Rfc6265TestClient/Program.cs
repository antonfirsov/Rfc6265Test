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
            HttpClientHandler handler = new HttpClientHandler();
            CookieContainer cookies = handler.CookieContainer;
            using HttpClient client = new HttpClient(handler);
            HttpResponseMessage response = await client.GetAsync("http://localhost:5000");

            foreach (Cookie cookie in cookies)
            {
                Console.WriteLine(cookie.ToString());
            }
        }
    }
}