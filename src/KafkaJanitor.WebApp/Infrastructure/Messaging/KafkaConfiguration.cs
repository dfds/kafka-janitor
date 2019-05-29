using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace KafkaJanitor.WebApp.Infrastructure.Messaging
{
    public class KafkaConfiguration
    {
        private const string KEY_PREFIX = "KAFKA_JANITOR_";
        private readonly IConfiguration _configuration;

        public KafkaConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string Key(string keyName) => string.Join("", KEY_PREFIX, keyName.ToUpper().Replace('.', '_'));

        private Tuple<string, string> GetConfiguration(string key)
        {
            var value = _configuration[Key(key)];

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return Tuple.Create(key, value);
        }

        public Dictionary<string, string> GetConfiguration()
        {
            var configurationKeys = new[]
            {
                "group.id",
                "enable.auto.commit",
                "bootstrap.servers",
                "broker.version.fallback",
                "api.version.fallback.ms",
                "ssl.ca.location",
                "sasl.username",
                "sasl.password",
                "sasl.mechanisms",
                "security.protocol",
            };

            //config.Add(new KeyValuePair<string, object>("request.timeout.ms", "3000"));

            return configurationKeys
                .Select(key => GetConfiguration(key))
                .Where(pair => pair != null)
                .ToDictionary(pair => pair.Item1, pair => pair.Item2);
        }
    }
}