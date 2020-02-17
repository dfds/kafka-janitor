using Tika.RestClient.Features.Acls;
using Tika.RestClient.Features.ApiKeys;
using Tika.RestClient.Features.ServiceAccounts;
using Tika.RestClient.Features.Topics;

namespace Specifications.TestDoubles
{
    public class TikaRestClientSpy : Tika.RestClient.IRestClient
    {
        public TikaTopicsClientSpy TikaTopicsClientSpy;

        public void Dispose()
        {
        }

        public TikaRestClientSpy()
        {
            TikaTopicsClientSpy = new TikaTopicsClientSpy();;
            Topics = TikaTopicsClientSpy;
        }
        public ITopicsClient Topics { get; }
        public IServiceAccountsClient ServiceAccounts { get; }
        public IApiKeysClient ApiKeys { get; }
        public IAclsClient Acls { get; }
    }
}