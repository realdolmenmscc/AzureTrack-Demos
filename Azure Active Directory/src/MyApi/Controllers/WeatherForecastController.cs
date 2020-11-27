using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

using MyApi.Models;

namespace MyApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        static readonly string[] ScopesRequiredByApiForReadData = new string[] { "data.read" };
        static readonly string[] ScopesRequiredByApiForWriteData = new string[] { "data.write" };

        public WeatherForecastController()
        {
        }

        [HttpGet]
        public IEnumerable<WeatherForecastModel> Get()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(ScopesRequiredByApiForReadData);

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecastModel
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecastModel weatherForecast)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(ScopesRequiredByApiForWriteData);

            Console.WriteLine("POSTED: " + weatherForecast.Summary);

            return Ok();
        }
    }
}
