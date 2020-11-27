using MoesTavern.Contracts;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoesTavern.Waiters.Integration
{
    public sealed class WaiterProxy
    {
        private static JsonSerializerOptions SerializerOptions { get; } = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private HttpClient Client { get; }

        public WaiterProxy(HttpClient client)
        {
            Client = client;
        }

        public async Task ServeDrinkAsync(Drink drink)
        {
            var ms = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(drink));
            ms.Seek(0, SeekOrigin.Begin);
            HttpContent content = new StreamContent(ms);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            await Client.PostAsync("serve", content);
        }

        public async Task<Order> GetNextOrderAsync()
        {
            HttpResponseMessage response = await Client.GetAsync("orders/next");

            if (response.IsSuccessStatusCode)
            {
                byte[] result = await response.Content.ReadAsByteArrayAsync();
                string json = System.Text.Encoding.UTF8.GetString(result);
                Console.WriteLine(json);

                return JsonSerializer.Deserialize<Order>(json, SerializerOptions);
            }

            return null;
        }
    }
}
