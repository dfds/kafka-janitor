using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace KafkaJanitor.RestApi.Features.Vault
{
    public class VaultHealthCheck : IHealthCheck
    {
        private readonly IVault _vault;
        private readonly ILogger _logger;

        public VaultHealthCheck(IVault vault, ILogger<VaultHealthCheck> logger)
        {
            _vault = vault;
            _logger = logger;
        }
        
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = new CancellationToken()
        )
        {
            try
            {
                await _vault.EnsureConnection();

                return HealthCheckResult.Healthy();
            }
            catch (TooManyUpdatesException)
            {
                return HealthCheckResult.Healthy();
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, "Could not connect to Vault");
                return HealthCheckResult.Unhealthy(
                    "Could not connect to Vault",
                    e
                ); 
            }    
        }
    }
}