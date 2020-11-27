using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AzureTrackMonitoringApi.Demo;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AzureTrackMonitoringApi.Controllers
{
    [ApiController]
    [Route("/")]
    public class DemoController : ControllerBase
    {
        private readonly ILogger<DemoController> _logger;
        private readonly Proxy _proxy;
        private readonly TelemetryClient _telemetryClient;
        private JsonSerializerOptions options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public DemoController(ILogger<DemoController> logger, Proxy proxy, TelemetryClient telemetryClient)
        {
            _logger = logger;
            _proxy = proxy;
            _telemetryClient = telemetryClient;
        }

        [HttpGet]
        public string Get()
        {
            _telemetryClient.TrackEvent("I'm okay");
            return "I'm okay!";
        }


        [HttpGet]
        [Route("/students")]
        public async Task<IEnumerable<Person>> GetPersonsAsync()
        {
            var persons = await GetPersons();
            _logger.LogInformation($"Found {persons.Count()} students to return to client");
            return persons;
        }

        [HttpGet]
        [Route("/teachers")]
        public async Task<IEnumerable<Person>> GetTeachersAsync()
        {
            // this call is fast
            await _proxy.GetAsync("https://reqres.in/api/teachers");
        
            // this one is slow
            var response = await _proxy.GetAsync("https://reqres.in/api/users?page=2&delay=3&cachebuster=" + DateTime.Now.Ticks);

            var persons = JsonSerializer.Deserialize<ReqResPeopleResponse>(response, options);

            return persons.data;
        }


        [HttpGet]
        [Route("/courses")]
        public async Task<IEnumerable<Record>> GetCoursesAsync()
        {
            var response = await _proxy.GetAsync("https://reqres.in/api/users");
            var persons = JsonSerializer.Deserialize<ReqResPeopleResponse>(response, options);
            return (IEnumerable<Record>) persons.data;
        }

        private async Task<List<Person>> GetPersons()
        {
            return new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Email="george.bluth@reqres.in",
                    FirstName="George",
                    LastName="Bluth",
                    Avatar="https://s3.amazonaws.com/uifaces/faces/twitter/calebogden/128.jpg"
                },
                new Person
                {
                    Id = 2,
                    Email="janet.weaver@reqres.in",
                    FirstName="Janet",
                    LastName="Weaver",
                    Avatar="https://s3.amazonaws.com/uifaces/faces/twitter/josephstein/128.jpg"
                },
                new Person
                {
                    Id = 3,
                    Email="emma.wong@reqres.in",
                    FirstName="Emma",
                    LastName="Wong",
                    Avatar="https://s3.amazonaws.com/uifaces/faces/twitter/olegpogodaev/128.jpg"
                },
                new Person
                {
                    Id = 4,
                    Email="eve.holt@reqres.in",
                    FirstName="Eve",
                    LastName="Holt",
                    Avatar="https://s3.amazonaws.com/uifaces/faces/twitter/marcoramires/128.jpg"
                },
                new Person
                {
                    Id = 5,
                    Email="charles.morris@reqres.in",
                    FirstName="Charles",
                    LastName="Morris",
                    Avatar="https://s3.amazonaws.com/uifaces/faces/twitter/stephenmoon/128.jpg"
                },
                new Person
                {
                    Id = 6,
                    Email="tracey.ramos@reqres.in",
                    FirstName="Tracey",
                    LastName="ramos",
                    Avatar="https://s3.amazonaws.com/uifaces/faces/twitter/bigmancho/128.jpg"
                }
            };
        }
    }
}
