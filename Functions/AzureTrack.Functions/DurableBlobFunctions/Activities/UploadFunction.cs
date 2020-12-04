using AzureTrack.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.IO;
using System.Threading.Tasks;

namespace AzureTrack.Functions.DurableBlobFunctions
{
    public sealed class UploadFunction
    {
        [FunctionName("UploadFunction")]
        public async Task Upload([ActivityTrigger] BlobModel input, Binder binder)
        {
            // Use a binder to dynamically set the binding output path
            using Stream fileOutputStream = await binder.BindAsync<Stream>(FunctionUtils.GetBindingAttributes(input.Analysis.Category, input.Name));
            await fileOutputStream.WriteAsync(input.Blob, 0, input.Blob.Length);
        }
    }
}
