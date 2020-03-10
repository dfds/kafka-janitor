using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Tika.RestClient;

namespace KafkaJanitor.RestApi.Features
{
    public class TikaHealthCheck : IHealthCheck
    {
        private readonly IRestClient _tikaClient;
        private readonly ILogger _logger;

        public TikaHealthCheck(IRestClient tikaClient, ILogger<TikaHealthCheck> logger)
        {
            _tikaClient = tikaClient;
            _logger = logger;
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
                _logger.Log(LogLevel.Error, exception, "Could not connect to Tika");
                return HealthCheckResult.Unhealthy(
                    "Could not connect to Tika",
                    exception
                );
            }
        }
    }
}