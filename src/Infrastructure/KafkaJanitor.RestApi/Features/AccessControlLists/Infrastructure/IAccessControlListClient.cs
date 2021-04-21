using System.Collections.Generic;
using System.Threading.Tasks;
using Tika.RestClient.Features.Acls.Models;

namespace KafkaJanitor.RestApi.Features.AccessControlLists.Infrastructure
{
    public interface IAccessControlListClient
    {
        Task CreateAclsForServiceAccount(string serviceAccountId, string prefix, string clusterId = null);
        Task DeleteAclsForServiceAccount(string serviceAccountId, string prefix, string clusterId = null);
        Task<IEnumerable<Acl>> GetAclsForServiceAccount(string serviceAccountId, string clusterId = null);
    }
}