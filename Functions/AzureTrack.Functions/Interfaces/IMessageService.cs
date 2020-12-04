using AzureTrack.Functions.Models;
using System.Threading.Tasks;

namespace AzureTrack.Functions
{
    public interface IMessageService
    {
        Task<string> SayHelloAsync(Person person);
        Task<Person[]> RetrievePersonsAsync();
    }
}
