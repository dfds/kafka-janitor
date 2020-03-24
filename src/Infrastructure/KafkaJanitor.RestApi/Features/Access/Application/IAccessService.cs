using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;

namespace KafkaJanitor.RestApi.Features.Access.Application
{
    public interface IAccessService
    {
        Task ProvideAccess(
            Capability cap,
            string topicPrefix
        );
    }
}