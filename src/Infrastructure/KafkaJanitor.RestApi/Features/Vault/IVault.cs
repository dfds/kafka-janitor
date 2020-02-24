using System;
using System.Threading.Tasks;
using KafkaJanitor.RestApi.Features.Topics.Domain.Models;
using KafkaJanitor.RestApi.Features.Vault.Model;

namespace KafkaJanitor.RestApi.Features.Vault
{
    public interface IVault
    {
        Task AddApiCredentials(Capability capability, ApiCredentials apiCredentials);
    }
}