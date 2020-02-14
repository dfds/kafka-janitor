using System.Threading.Tasks;
using KafkaJanitor.RestClient.Features.Access.Models;

namespace KafkaJanitor.RestClient.Features.Access
{
    public interface IAccessClient
    {
        Task RequestAsync(ServiceAccountRequestInput input);
    }
}