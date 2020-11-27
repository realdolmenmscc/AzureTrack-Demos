using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyWebApp.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IConfiguration _configuration;

        public ApiService(IHttpClientFactory clientFactory,
                          ITokenAcquisition tokenAcquisition,
                          IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _tokenAcquisition = tokenAcquisition;
            _configuration = configuration;
        }

        public async Task<JArray> GetApiDataAsync()
        {
            var client = await GetHttpClientAsync("Data.Read");
            var response = await client.GetAsync("api/weatherforecast");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JArray.Parse(responseContent);

                return data;
            }

            //TODO: logging and filter to catch these execptions to give the user a correct fault message
            throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");

        }

        public async Task<bool> PostApiDataAsync()
        {
            var client = await GetHttpClientAsync("Data.Write");
            var body = JsonConvert.SerializeObject(new { Date = DateTime.Now, TemperatureC = 10, Summary = "Test post" });
            var response = await client.PostAsync("api/weatherforecast", new StringContent(body, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            //TODO: logging and filter to catch these execptions to give the user a correct fault message
            throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");

        }

        public async Task<JArray> GetApiDataUsersAsync()
        {
            var client = await GetHttpClientAsync(string.Empty);
            var response = await client.GetAsync("api/users");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JArray.Parse(responseContent);

                return data;
            }

            //TODO: logging and filter to catch these execptions to give the user a correct fault message
            throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
        }

        private async Task<HttpClient> GetHttpClientAsync(string neededScope)
        {
            var client = _clientFactory.CreateClient();

            var scope = _configuration.GetSection("Api:ScopesForAccessToken").GetValue<string>(neededScope);
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { scope });

            client.BaseAddress = new Uri(_configuration["Api:ApiBaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}
