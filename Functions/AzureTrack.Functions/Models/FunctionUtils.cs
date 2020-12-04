using Microsoft.Azure.WebJobs;
using System;
using System.IO;

namespace AzureTrack.Functions.Models
{
    public static class FunctionUtils
    {
        public static Attribute[] GetBindingAttributes(string category, string name)
            => new Attribute[]
                {
                    new BlobAttribute($"{category}/{name}", FileAccess.Write),
                    new StorageAccountAttribute("BlobStorageConnection")
                };
    }
}
