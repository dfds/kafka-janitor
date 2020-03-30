using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using KafkaJanitor.RestClient;
using KafkaJanitor.RestClient.Factories;
using KafkaJanitor.RestClient.Features.Access.Models;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace KafkaJanitor.IntegrationTests
{
    public class AccessInSuccessionScenario
    {
        private IHost testHost;
        private IRestClient restClient;

        private ServiceAccountRequestInput serviceAccountRequestInput;
        private List<Task> _tasks = new List<Task>();

        [Fact]
        public async Task AccessInSuccessionScenarioRecipe()
        {
                  Given_a_rest_client();
                  When_access_is_requested();
                  And_same_access_is_requested();
            await Then_no_exceptions_are_thrown();
        }

        private void Given_a_rest_client()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:5000");
            restClient = RestClientFactory.Create(httpClient);
        }

        private void When_access_is_requested()
        {
            var capabilityName =
                "test-capability-name-" +
                Guid.NewGuid().ToString()
                    .Substring(0, 5);

            serviceAccountRequestInput = new ServiceAccountRequestInput
            {
                CapabilityId = Guid.NewGuid().ToString(),
                CapabilityName = capabilityName,
                TopicPrefix = capabilityName,
                CapabilityRootId = "root-id-" + capabilityName
            };

            _tasks.Add(restClient.Access.RequestAsync(serviceAccountRequestInput));
        }

        private async Task And_same_access_is_requested()
        {
            _tasks.Add(restClient.Access.RequestAsync(serviceAccountRequestInput));
        }

        private async Task Then_no_exceptions_are_thrown()
        {
            await Task.WhenAll(_tasks);
        }
    }
}