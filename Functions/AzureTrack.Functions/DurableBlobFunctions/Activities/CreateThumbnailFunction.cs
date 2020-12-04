using AzureTrack.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Threading.Tasks;

namespace AzureTrack.Functions.DurableBlobFunctions
{
    public sealed class CreateThumbnailFunction
    {
        [FunctionName("CreateThumbnailFunction")]
        public async Task CreateThumbnail([ActivityTrigger] BlobModel input, Binder binder)
        {
            using Stream thumbnail = await binder.BindAsync<Stream>(FunctionUtils.GetBindingAttributes($"{input.Analysis.Category}-thumbs", input.Name));

            using Image<Rgba32> image = Image.Load(input.Blob);
            image.Mutate(i =>
            {
                i.Resize(340, 0);
                int height = i.GetCurrentSize().Height;
                i.Crop(new Rectangle(0, 0, 340, height < 226 ? height : 226));
            });

            image.SaveAsJpeg(thumbnail);
        }
    }
}
