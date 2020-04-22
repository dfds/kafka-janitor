using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace KafkaJanitor.RestApi.Features.Vault.RestfulVault.Infrastructure
{
    [Route("api/vault")]
    public class VaultController : ControllerBase
    {
        private readonly InMemoryVault _inMemoryVault;

        public VaultController(InMemoryVault inMemoryVault)
        {
            _inMemoryVault = inMemoryVault;
        }
        [HttpGet("")]
        public IActionResult GetAllAsync()
        {
            return Ok(_inMemoryVault.RootIdToApiCredentials);
        }
    }
}