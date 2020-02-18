using System.Threading.Tasks;

namespace KafkaJanitor.RestApi.Features.AccessControlLists.Infrastructure
{
    public interface IAccessControlListClient
    {
        Task CreateAclsForServiceAccount(string serviceAccountId, string capabilityName);
    }
}