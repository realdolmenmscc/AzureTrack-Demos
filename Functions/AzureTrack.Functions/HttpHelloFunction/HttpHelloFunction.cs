using AzureTrack.Functions.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureTrack.Functions
{
    public class HttpHelloFunction
    {
        public string ConnectionString { get; set; }
        public IMessageService MessageService { get; set; }

        public HttpHelloFunction(IMessageService messageService, IConfiguration configuration)
        {
            MessageService = messageService;
            ConnectionString = configuration.GetValue<string>("ConnectionString");
        }

        [FunctionName("HttpHelloFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] Person person,
            ILogger log)
        {
            log.LogInformation("HttpHello function connected {0}!", ConnectionString);

            string message = await MessageService.SayHelloAsync(person);

            return new OkObjectResult(message);
        }
    }
}
