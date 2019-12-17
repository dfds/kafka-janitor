using System;
using System.Threading.Tasks;
using Tika.Client.Models;
using Xunit;

namespace Tika.Client.IntegrationTests.Features.ApiKeys
{
    public class CreateDeleteServiceApiKeyScenario
    {
        private TikaClient _tikaClient;
        private ServiceAccount _serviceAccount;
        private ApiKeySet _apiKey;

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
            var baseUri = new Uri("http://localhost:3000", System.UriKind.Absolute);
            _tikaClient = TikaClient.FromBaseUri(baseUri);
        }

        private async Task And_a_service_account()
        {
            var serviceAccountName = "test-" + Guid.NewGuid().ToString().Substring(0, 5);
            _serviceAccount = await _tikaClient.ServiceAccounts.CreateAsync(
                serviceAccountName,
                "Created by integration test, may be deleted at any time."
            );
        }

        private async Task When_a_api_key_is_created()
        {
            _apiKey = await _tikaClient.ApiKeys.Create(_serviceAccount.Id,
                "Created by integration test, may be deleted at any time.");
        }

        private async Task Then_it_should_be_in_the_api_keys_list()
        {
            var serviceAccounts = await _tikaClient.ApiKeys.GetAsync();

            Assert.Contains(serviceAccounts, a =>
                a.Key == _apiKey.Key
            );
        }

        private async Task When_a_apy_key_is_deleted()
        {
            await _tikaClient.ApiKeys.DeleteAsync(_apiKey.Key);
        }

        private async Task Then_it_should_not_be_in_the_api_key_list()
        {
            var serviceAccounts = await _tikaClient.ApiKeys.GetAsync();

            Assert.DoesNotContain(serviceAccounts, a =>
                a.Key == _apiKey.Key
            );
        }
    }
}