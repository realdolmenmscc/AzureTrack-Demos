using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace AzureTrack.BlobStorage.Api
{
    public sealed class BlobService
    {
        private BlobServiceClient Client { get; }
        private StorageSharedKeyCredential Credential { get; }

        public BlobService(BlobServiceClient client, StorageSharedKeyCredential credential)
        {
            Client = client;
            Credential = credential;
        }

        /// <summary>
        /// Retrieve blob collection metadata from the container
        /// </summary>
        public BlobModel[] GetContainerItems(string containerName)
        {
            try
            {
                BlobContainerClient container = GetBlobContainer(containerName);
                Pageable<BlobItem> blobs = container.GetBlobs();

                return blobs.Select(blob => new BlobModel
                {
                    Name = blob.Name.Replace("/", "_"),
                    Category = containerName,
                    Url = GetSasUri(container, blob)
                }).ToArray();
            }
            catch (RequestFailedException)
            {
                return Array.Empty<BlobModel>();
            }
        }



        /// <summary>
        /// Generate the SAS token
        /// </summary>
        private string GetSasUri(BlobContainerClient container, string blobName)
        {
            // Create a SAS token that's valid for two minutes.
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = container.Name,
                BlobName = blobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1),
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(2)
            };

            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

            // Use the key to get the SAS token.
            string sasToken = sasBuilder.ToSasQueryParameters(Credential).ToString();

            return $"{container.Uri.AbsoluteUri}/{blobName}?{sasToken}";
        }


        #region Demo Hideout

        public BlobContainerClient GetBlobContainer(string name)
           => Client.GetBlobContainerClient(name);

        public string GetSasUri(string container, string name)
        {
            BlobContainerClient containerClient = Client.GetBlobContainerClient(container);

            return GetSasUri(containerClient, name);
        }

        private string GetSasUri(BlobContainerClient container, BlobItem blobItem)
            => GetSasUri(container, blobItem.Name);

        #endregion Demo Hideout
    }
}
