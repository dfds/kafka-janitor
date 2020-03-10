using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KafkaJanitor.RestApi.Features.Vault
{
    public class VaultHealthCheck : IHealthCheck
    {
        private readonly IVault _vault;

        public VaultHealthCheck(IVault vault)
        {
            _vault = vault;
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
            catch (Exception e)
            {
                return HealthCheckResult.Unhealthy(
                    $"Could not connect to Vault\nException: {e.Message}",
                    e
                ); 
            }    
        }
    }
}