using System;

namespace KafkaJanitor.RestApi.Features.Vault.Model
{
    public class InvalidVaultConfigurationException : Exception
    {
        public InvalidVaultConfigurationException() : base("An invalid configuration was provided to Vault")
        {
            
        }

        public InvalidVaultConfigurationException(string message) : base($"{message} was not configured as expected for Vault")
        {
            
        }
    }
}