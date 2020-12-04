using AzureTrack.Functions.Models;
using ImpromptuInterface;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureTrack.Functions.DurableInvoices
{
    public static class FanOutFanInFunction
    {
        [FunctionName("FanOutFanIn")]
        public static async Task Run(
             [OrchestrationTrigger] IDurableOrchestrationContext context,
             ILogger logger)
        {
            Invoice[] invoices = await context.CallActivityAsync<Invoice[]>("GatherInvoicesFunction", null);

            var tasks = new Task[invoices.Length];

            logger.LogInformation("Orders to be processed: {0}", invoices.Length);

            for (int i = 0; i < invoices.Length; i++)
            {
                tasks[i] = context.CallActivityAsync("ProcessInvoiceFunction", invoices[i]);
            }

            await Task.WhenAll(tasks);

            await context.CallActivityAsync("SendInvoicesFunction", invoices);
        }
    }
}
