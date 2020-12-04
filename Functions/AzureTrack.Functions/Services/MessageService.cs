using AzureTrack.Functions.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureTrack.Functions.Services
{
    public sealed class MessageService : IMessageService
    {
        private ILogger<MessageService> Logger { get; }

        public MessageService(ILogger<MessageService> logger)
        {
            Logger = logger;
        }

        public async Task<Person[]> RetrievePersonsAsync()
        {
            await Task.Delay(5000);

            return new[]
            {
                new Person{ Name="Jef" },
                new Person{ Name="Jos" },
                new Person{ Name="François" },
                new Person{ Name="Julien" },
                new Person{ Name="Modest" },
            };
        }

        public async Task<string> SayHelloAsync(Person person)
        {
            Logger.LogInformation("Saying hello to {0}", person?.Name ?? "Unknown");

            await Task.Delay(2000);

            return string.IsNullOrEmpty(person.Name)
                    ? "HttpHello! Pass a name in the request body for a personalized response."
                    : $"Hello, {person.Name}!";
        }
    }
}
