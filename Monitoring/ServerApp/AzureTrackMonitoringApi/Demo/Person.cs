using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzureTrackMonitoringApi.Demo
{
    public class Person
    {
        public int Id { get; set; }
        public string Email { get; set; }
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
        public string Avatar { get; set; }
    }

    public class ReqResPeopleResponse
    {
        public IEnumerable<Person> data { get; set; }
    }
}
