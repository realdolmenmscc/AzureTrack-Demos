using AzureTrack.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureTrack.Functions.DurableInvoices
{
    public static class SendInvoicesFunction
    {
        [FunctionName("SendInvoicesFunction")]
        public static void Run([ActivityTrigger] Invoice[] invoices, ILogger log)
        {
            log.LogInformation($"Sending invoices...");
        }
    }
}