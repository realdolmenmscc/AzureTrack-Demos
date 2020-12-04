using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureTrack.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Weakly;

namespace AzureTrack.Functions.DurableBlobFunctions
{
    public sealed class CreateZipFunction
    {
        private static WeakValueDictionary<string, SemaphoreSlim> Semaphores = new WeakValueDictionary<string, SemaphoreSlim>();
        private BlobServiceClient Client { get; }
        public CreateZipFunction(BlobServiceClient client)
        {
            Client = client;
        }

        [FunctionName("CreateZipFunction")]
        public async Task ZipIt([ActivityTrigger] BlobModel input, Binder binder)
        {
            SemaphoreSlim semaphore = GetSemaphoreForContainer(input.Analysis.Category);
            await semaphore.WaitAsync();
            try
            {
                string name = $"{input.Analysis.Category}.zip";
                using Stream zipStream = await binder.BindAsync<Stream>(FunctionUtils.GetBindingAttributes("zip-collection", name));
                using var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true);
                BlobContainerClient containerClient = GetCloudBlobContainer(input.Analysis.Category);
                containerClient.DeleteBlobIfExists(name);

                Pageable<BlobItem> blobs = containerClient.GetBlobs();

                foreach (BlobItem blob in blobs)
                {
                    BlobClient client = containerClient.GetBlobClient(blob.Name);
                    using var blobStream = new MemoryStream();
                    client.DownloadTo(blobStream);
                    using Stream entryStream = archive.CreateEntry(blob.Name).Open();
                    blobStream.Seek(0, SeekOrigin.Begin);
                    blobStream.CopyTo(entryStream);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        private BlobContainerClient GetCloudBlobContainer(string name)
        {
            BlobContainerClient container = Client.GetBlobContainerClient(name);
            container.CreateIfNotExists();
            return container;
        }

        private SemaphoreSlim GetSemaphoreForContainer(string container)
        {
            lock (Semaphores)
            {
                var semaphore = Semaphores.GetValueOrDefault(container);
                if (semaphore == null)
                {
                    semaphore = new SemaphoreSlim(1, 1);
                    Semaphores.Add(container, semaphore);
                }
                return semaphore;
            }
        }
    }
}
