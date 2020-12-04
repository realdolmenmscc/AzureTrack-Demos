using AzureTrack.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.IO;
using System.Threading.Tasks;

namespace AzureTrack.Functions.DurableBlobFunctions
{
    public class UploadStarterFunction
    {
        [FunctionName("UploadStarterFunction")]
        public async Task StartOrchestratorBlobTrigger(
            [BlobTrigger("uploads/{name}", Connection = "BlobStorageConnection")] Stream blob,
            string name,
            [DurableClient] IDurableOrchestrationClient starter)
        {
            BinaryReader binaryReader = new BinaryReader(blob);
            var input = new BlobModel
            {
                Blob = binaryReader.ReadBytes((int)blob.Length),
                Name = name
            };
            await starter.StartNewAsync("UploadOrchestratorFunction", input);
        }
    }
}