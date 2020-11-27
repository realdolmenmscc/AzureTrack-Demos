using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace AzureTrack.BlobStorage.Uploader
{
    class Program
    {
        static IConfiguration Config { get; } = Configuration.Initialize();
        static BlobServiceClient BlobServiceClient { get; } = new BlobServiceClient(Config["BlobStorageconnection"]);

        static void Main(string[] args)
        {
            Console.WriteLine("Uploader started");
            string directory = Path.Combine(Environment.CurrentDirectory, "uploads");

            Directory.CreateDirectory(directory);

            Cleanup();

            Console.WriteLine("Listening on files dropped in: {0}", directory);

            using var fileSystemWatcher = new FileSystemWatcher
            {
                Filter = "*.jpg",
                Path = directory
            };

            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.EnableRaisingEvents = true;

            Console.ReadLine();
        }

        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            // Get Container reference
            BlobContainerClient container = BlobServiceClient.GetBlobContainerClient("uploads");
            container.CreateIfNotExists();

            // Get reference to the blob
            BlobClient blob = container.GetBlobClient(e.Name);

            // Delete the blob if it already exists
            blob.DeleteIfExists();

            // Upload file to blob
            Console.WriteLine("Uploading to BlobStorage: {0}", e.FullPath);
            blob.Upload(e.FullPath);

            // Delete file
            File.Delete(e.FullPath);
        }



        private static void Cleanup()
        {
            // For demo purposes we'll want to start with a clean slate.
            // Delete all of the following containers
            var containers = new string[]
            {
                "uploads",
                "cars",
                "cars-thumbs",
                "cats",
                "cats-thumbs",
                "dogs",
                "dogs-thumbs",
                "random",
                "random-thumbs",
                "zip-collection",
                "rejected"
            };

            foreach (var container in BlobServiceClient.GetBlobContainers().Where(p => containers.Contains(p.Name)))
            {
                BlobServiceClient.DeleteBlobContainer(container.Name);
            }
        }
    }
}
