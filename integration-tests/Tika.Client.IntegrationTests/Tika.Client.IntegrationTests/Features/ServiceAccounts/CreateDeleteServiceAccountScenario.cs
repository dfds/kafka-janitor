using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tika.Client.Models;
using Xunit;

namespace Tika.Client.IntegrationTests.Features.ServiceAccounts
{
    public class CreateDeleteServiceAccountScenario
    {
        private TikaClient _tikaClient;
        private ServiceAccount _serviceAccount;

        [Fact]
        public async Task CreateDeleteServiceAccountScenarioRecipe()
        { 
            Given_a_tika_client();
            await When_a_service_account_is_created();
            await Then_it_should_be_in_the_service_accounts_list();
            await When_a_service_account_is_deleted();
            await Then_it_should_not_be_in_the_service_accounts_list();
        }

        private void Given_a_tika_client()
        {
            var httpClient = new HttpClient();
            var options = new TikaOptions { TIKA_API_ENDPOINT = "http://localhost:3000"};
            httpClient.BaseAddress = new Uri(options.TIKA_API_ENDPOINT, System.UriKind.Absolute);
            _tikaClient = new TikaClient(httpClient, options);
        }

        private async Task When_a_service_account_is_created()
        {
            var serviceAccountName = "test-" + Guid.NewGuid().ToString().Substring(0, 5);
            _serviceAccount = await _tikaClient.CreateServiceAccount(
                serviceAccountName,
                "Created by integration test, may be deleted at any time."
            );
        }

        private async Task Then_it_should_be_in_the_service_accounts_list()
        {
            var serviceAccounts = await _tikaClient.GetServiceAccounts();

            Assert.Contains(serviceAccounts, s =>
                s.Id == _serviceAccount.Id &&
                s.Name == _serviceAccount.Name &&
                s.Description == _serviceAccount.Description
            );
        }

        private async Task When_a_service_account_is_deleted()
        {
            await _tikaClient.DeleteServiceAccount(_serviceAccount.Id);
        }

        private async Task Then_it_should_not_be_in_the_service_accounts_list()
        {
            var serviceAccounts = await _tikaClient.GetServiceAccounts();

            Assert.DoesNotContain(serviceAccounts, s =>
                s.Id == _serviceAccount.Id &&
                s.Name == _serviceAccount.Name &&
                s.Description == _serviceAccount.Description
            );
        }
    }
}