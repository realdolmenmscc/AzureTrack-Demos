using AzureTrack.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AzureTrack.Functions.DurableInvoices
{
    public static class ProcessInvoiceFunction
    {
        private static Random Random = new Random();

        [FunctionName("ProcessInvoiceFunction")]
        public static async Task Run(
            [ActivityTrigger] Invoice invoice,
            ILogger logger)
        {
            // Some random processing of tasks..
            await Task.Delay(Random.Next(0, 3000));

            logger.LogInformation("Processed invoice (id: {0}, client: {1})", invoice.Id, invoice.ClientName);
        }
    }
}
