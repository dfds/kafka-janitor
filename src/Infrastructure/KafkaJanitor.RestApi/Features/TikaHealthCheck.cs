using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Tika.RestClient;

namespace KafkaJanitor.RestApi.Features
{
    public class TikaHealthCheck : IHealthCheck
    {
        private readonly IRestClient _tikaClient;

        public TikaHealthCheck(IRestClient tikaClient)
        {
            _tikaClient = tikaClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = new CancellationToken()
        )
        {
            try
            {
                await _tikaClient.Topics.GetAllAsync();

                return HealthCheckResult.Healthy();
            }
            catch (Exception exception)
            {
                return HealthCheckResult.Unhealthy(
                    "Could not Connect to Tika",
                    exception
                );
            }
        }
    }
}