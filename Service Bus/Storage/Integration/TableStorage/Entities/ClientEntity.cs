using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperApp.Integration.TableStorage.Entities
{
    public class ClientEntity : TableEntity
    {
        public ClientEntity()
        {}

        public ClientEntity(string osName, string browserName)
        {
            PartitionKey = osName;
            RowKey = browserName;
        }

        public int CombinationCount { get; set; }
    }
}
