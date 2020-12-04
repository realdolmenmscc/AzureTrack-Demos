using Azure.Storage.Blobs;
using AzureTrack.Functions.Services;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AzureTrack.Functions.Startup))]
namespace AzureTrack.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Keep this configuration as light as possible.
            // Never misuse the context to resolve services prematurely while configuring.

            FunctionsHostBuilderContext context = builder.GetContext();

            builder.Services.AddScoped<IMessageService, MessageService>();

            builder.Services.AddApplicationInsightsTelemetry();

            builder.Services.AddSingleton(p => new BlobServiceClient(context.Configuration.GetValue<string>("BlobStorageconnection")));

            builder.Services.AddSingleton(p => new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(context.Configuration.GetValue<string>("ComputerVisionApiKey")))
            {
                Endpoint = context.Configuration.GetValue<string>("ComputerVisionEndpoint")
            });
        }
    }
}
