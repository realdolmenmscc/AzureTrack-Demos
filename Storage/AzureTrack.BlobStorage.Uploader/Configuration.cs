using Microsoft.Extensions.Configuration;

namespace AzureTrack.BlobStorage.Uploader
{
    internal static class Configuration
    {
        public static IConfiguration Initialize()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                             .AddUserSecrets<Program>()
                                             .Build();
        }
    }
}
