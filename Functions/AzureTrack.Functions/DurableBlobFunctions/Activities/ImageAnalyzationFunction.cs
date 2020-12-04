using AzureTrack.Functions.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureTrack.Functions.DurableBlobFunctions
{
    public class ImageAnalyzationFunction
    {
        private ComputerVisionClient VisionClient { get; }

        private static readonly List<VisualFeatureTypes?> Features = new List<VisualFeatureTypes?>
        {
            VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
            VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Tags, VisualFeatureTypes.Brands, VisualFeatureTypes.Adult
        };

        public ImageAnalyzationFunction(ComputerVisionClient visionClient)
        {
            VisionClient = visionClient;
        }

        [FunctionName("ImageAnalyzationFunction")]
        public async Task<AnalysisResult> Analyze([ActivityTrigger] BlobModel input)
        {
            using var image = new MemoryStream(input.Blob);
            ImageAnalysis analysis = await VisionClient.AnalyzeImageInStreamAsync(image, Features);

            return new AnalysisResult
            {
                Category = AnalyseCategory(analysis),
                Brand = AnalyseBrand(analysis),
                IsRejectedContent = IsRejectedContent(analysis)
            };
        }

        private static bool IsRejectedContent(ImageAnalysis image)
            => image.Adult.IsAdultContent || image.Adult.IsGoryContent || image.Adult.IsRacyContent;

        private static bool IsDog(ImageAnalysis image)
            => image.Categories.Any(p => p.Name.Contains("dog"));

        private static bool IsCat(ImageAnalysis image)
            => image.Categories.Any(p => p.Name.Contains("cat"));

        private static bool IsCar(ImageAnalysis image)
            => image.Categories.Any(p => p.Name.Contains("car"));

        private static string AnalyseCategory(ImageAnalysis image)
        {
            if (IsCar(image))
                return "cars";

            if (IsCat(image))
                return "cats";

            if (IsDog(image))
                return "dogs";

            return "random";
        }

        private static string AnalyseBrand(ImageAnalysis image)
        {
            if (image.Brands.Count == 0)
                return null;

            double maxBrand = image.Brands.Max(p => p.Confidence);
            return image.Brands.Where(p => p.Confidence == maxBrand)
                               .Select(p => p.Name)
                               .FirstOrDefault();
        }
    }
}
