using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics;

namespace KafkaJanitor.RestClient.Features.Access
{
    public interface IAccessClient
    {
        Task RequestAsync(ServiceAccountRequestInput input);
    }
}