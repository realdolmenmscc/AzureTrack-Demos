using MoesTavern.Contracts;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoesTavern.Customers.Integration
{
    public sealed class CustomerProxy
    {
        private static JsonSerializerOptions SerializerOptions { get; } = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        private HttpClient Client { get; }

        public CustomerProxy(HttpClient httpClient)
        {
            Client = httpClient;
        }

        public async Task TakeASeatAsync(string customerName)
        {
            HttpContent content = new ByteArrayContent(Array.Empty<byte>());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            await Client.PutAsync(customerName, content);
        }

        public async Task LeaveBarAsync(string customerName)
        {
            await Client.DeleteAsync(customerName);
        }

        public async Task SendOrderAsync(Order order)
        {
            var ms = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes<Order>(order, SerializerOptions));
            ms.Seek(0, SeekOrigin.Begin);
            HttpContent content = new StreamContent(ms);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            await Client.PostAsync("order", content);
        }

        public async Task<Drink> GetNextDrinkAsync(string customerName)
        {
            HttpResponseMessage response = await Client.GetAsync($"{customerName}/next");

            if (response.IsSuccessStatusCode)
            {
                byte[] result = await response.Content.ReadAsByteArrayAsync();

                return JsonSerializer.Deserialize<Drink>(result, SerializerOptions);
            }

            return null;
        }
    }
}