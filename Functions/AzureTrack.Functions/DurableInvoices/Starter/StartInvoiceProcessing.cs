using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureTrack.Functions.DurableInvoices
{
    public static class StartInvoiceProcessing
    {
        /*  
         *  Using a TimerTrigger to start the durable function orchestrator.
         *  The TimerTrigger is scheduled to run every 5 mins.
         *  The actual CRON expression is configured in local.settings.json
         */

        [FunctionName("StartInvoiceProcessing")]
        public static Task Run(
            [TimerTrigger("%StartInvoiceProcessingSchedule%")] TimerInfo timerInfo,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger logger)
        {
            logger.LogInformation("Starter function triggered");
            return starter.StartNewAsync("FanOutFanIn", null);
        }
    }
}
