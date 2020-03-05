using System;
using System.Collections.Generic;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KafkaJanitor.RestApi.Enablers.PrometheusHealthCheck
{
    public static class HealthChecks
    {
        static HealthChecks()
        {
            Checks = new Dictionary<string, Func<HealthCheckResult>>
            {
                {"assembly", () => HealthCheckResult.Healthy()}
            };
        }

        public static Dictionary<string, Func<HealthCheckResult>> Checks;
    }
}