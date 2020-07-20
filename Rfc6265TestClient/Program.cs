using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rfc6265TestClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using HttpClient client = new HttpClient();
            string result = await client.GetStringAsync("http://localhost:5000");
            Console.WriteLine(result);
        }
    }
}