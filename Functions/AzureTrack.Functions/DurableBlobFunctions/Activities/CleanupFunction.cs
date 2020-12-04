using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureTrack.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace AzureTrack.Functions.DurableBlobFunctions
{
    public sealed class CleanupFunction
    {
        private BlobServiceClient Client { get; }
        public CleanupFunction(BlobServiceClient client)
        {
            Client = client;
        }

        [FunctionName("CleanupFunction")]
        public async Task Cleanup([ActivityTrigger] BlobModel input)
        {
            var containerClient = Client.GetBlobContainerClient("uploads");
            string name = input.Name.Split("/").LastOrDefault();
            await containerClient.DeleteBlobIfExistsAsync(name);
        }
    }
}
