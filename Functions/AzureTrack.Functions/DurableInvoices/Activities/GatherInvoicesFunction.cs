using AzureTrack.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureTrack.Functions.DurableInvoices
{
    public static class GatherInvoicesFunction
    {
        [FunctionName("GatherInvoicesFunction")]
        public static Task<Invoice[]> Run(
            [ActivityTrigger] IDurableActivityContext context,
            ILogger logger)
        {
            logger.LogInformation("Gathering invoices...");
            // Random Belgian companies from Forbes 2019
            return Task.FromResult(new[]
            {
                new Invoice{ Id = 1 , ClientName= "Dexia"},
                new Invoice{ Id = 2 , ClientName= "Umicore"},
                new Invoice{ Id = 3 , ClientName= "Colruyt Group"},
                new Invoice{ Id = 4 , ClientName= "Proximus Group"},
                new Invoice{ Id = 5 , ClientName= "Aveve"},
                new Invoice{ Id = 6,  ClientName= "Kinepolis"}
            });
        }
    }
}
