using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureTrackMonitoringApi.Demo
{
  public class Proxy
  {
    private HttpClient _httpClient;

    public Proxy(HttpClient httpClient)
    {
      _httpClient = httpClient;
    }

    public async Task<string> GetAsync(string url)
    {
      return await (await _httpClient.GetAsync(url)).Content.ReadAsStringAsync();
    }
  }
}
