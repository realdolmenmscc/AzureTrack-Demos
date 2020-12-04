using AzureTrack.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureTrack.Functions.DurableBlobFunctions
{
    public class UploadOrchestratorFunction
    {
        [FunctionName("UploadOrchestratorFunction")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            BlobModel blob = context.GetInput<BlobModel>();
            blob.ApplyAnalysis(await context.CallActivityAsync<AnalysisResult>("ImageAnalyzationFunction", blob));

            var tasks = new List<Task>();

            // Using a FanOut/FanIn strategy to process all activities in parrallel
            AddThumbnailTask(tasks, context, blob);
            AddUploadTask(tasks, context, blob);
            AddZipTask(tasks, context, blob);

            await Task.WhenAll(tasks);

            await context.CallActivityAsync("CleanupFunction", blob);
        }

        private static void AddThumbnailTask(List<Task> tasks, IDurableOrchestrationContext context, BlobModel blob)
        {
            if (blob.Analysis.IsRejectedContent)
                return;

            tasks.Add(context.CallActivityAsync<AnalysisResult>("CreateThumbnailFunction", blob));
        }

        private static void AddUploadTask(List<Task> tasks, IDurableOrchestrationContext context, BlobModel blob)
        {
            tasks.Add(context.CallActivityAsync<AnalysisResult>("UploadFunction", blob));
        }

        private static void AddZipTask(List<Task> tasks, IDurableOrchestrationContext context, BlobModel blob)
        {
            if (blob.Analysis.IsRejectedContent)
                return;

            tasks.Add(context.CallActivityAsync<AnalysisResult>("CreateZipFunction", blob));
        }
    }
}
