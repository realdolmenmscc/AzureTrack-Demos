using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using SuperApp.Integration.TableStorage.Entities;
using SuperApp.Models;
using System.Threading.Tasks;

namespace SuperApp.Integration.TableStorage
{
    public sealed class StorageProxy
    {
        private string StorageConnectionString { get; }

        public StorageProxy(IConfiguration confg)
        {
            StorageConnectionString = confg["ConnectionStrings:MySuperStorageAccount"];
        }

        internal async Task<ClientResult> GetClientResultAsync(string osName, string browserName)
        {
            CloudTable table = await GetTableAsync();

            TableOperation operation = TableOperation.Retrieve<ClientEntity>(osName, browserName);
            TableResult result = await table.ExecuteAsync(operation);

            if (result.Result is ClientEntity entity)
            {
                return new ClientResult
                {
                    OsName = entity.PartitionKey,
                    BrowserName = entity.RowKey,
                    CombinationCount = entity.CombinationCount
                };
            }

            return null;
        }

        private async Task<CloudTable> GetTableAsync()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference("LoggedBrowsers");
            await table.CreateIfNotExistsAsync();
            return table;
        }

        internal async Task SaveClientResultAsync(ClientResult result)
        {
            CloudTable table = await GetTableAsync();

            var entity = new ClientEntity(result.OsName, result.BrowserName)
            {
                CombinationCount = result.CombinationCount
            };

            TableOperation operation = TableOperation.InsertOrMerge(entity);
            await table.ExecuteAsync(operation);
        }
    }
}
