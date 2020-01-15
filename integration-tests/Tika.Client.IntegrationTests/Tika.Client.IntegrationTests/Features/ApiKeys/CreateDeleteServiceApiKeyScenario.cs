using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tika.Client.Models;
using Xunit;

namespace Tika.Client.IntegrationTests.Features.ApiKeys
{
    public class CreateDeleteServiceApiKeyScenario
    {
        private TikaClient _tikaClient;
        private ServiceAccount _serviceAccount;
        private ApiKey _apiKey;

        [Fact]
        async Task CreateDeleteServiceApiKeyScenarioRecipe()
        {
            Given_a_tika_client();
            await And_a_service_account();
            await When_a_api_key_is_created();
            await Then_it_should_be_in_the_api_keys_list();
            await When_a_apy_key_is_deleted();
            await Then_it_should_not_be_in_the_api_key_list();
        }

        private void Given_a_tika_client()
        {
            var httpClient = new HttpClient();
            var options = new TikaOptions { TIKA_API_ENDPOINT = "http://localhost:3000"};
            httpClient.BaseAddress = new Uri(options.TIKA_API_ENDPOINT, System.UriKind.Absolute);
            _tikaClient = new TikaClient(httpClient, options);
        }

        private async Task And_a_service_account()
        {
            var serviceAccountName = "test-" + Guid.NewGuid().ToString().Substring(0, 5);
            _serviceAccount = await _tikaClient.CreateServiceAccount(
                serviceAccountName,
                "Created by integration test, may be deleted at any time."
            );
        }

        private async Task When_a_api_key_is_created()
        {
            _apiKey = await _tikaClient.CreateApiKey(_serviceAccount.Id.ToString(),
                "Created by integration test, may be deleted at any time.");
        }

        private async Task Then_it_should_be_in_the_api_keys_list()
        {
            var serviceAccounts = await _tikaClient.GetApiKeys();

            Assert.Contains(serviceAccounts, a =>
                a.Key == _apiKey.Key
            );
        }

        private async Task When_a_apy_key_is_deleted()
        {
            await _tikaClient.DeleteApiKey(_apiKey.Key);
        }

        private async Task Then_it_should_not_be_in_the_api_key_list()
        {
            var serviceAccounts = await _tikaClient.GetApiKeys();

            Assert.DoesNotContain(serviceAccounts, a =>
                a.Key == _apiKey.Key
            );
        }
    }
}