using System.Threading.Tasks;
using KafkaJanitor.RestApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace Specifications
{
    public static class RestApiHostCreator
    {
        public static async Task<IHost> CreateAsync()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<Startup>();
                    // Look for controllers in the same assembly as the startup class
                    webHost.UseSetting(WebHostDefaults.ApplicationKey, typeof(Startup).Assembly.GetName().Name);
                });

            var host = await hostBuilder.StartAsync();
            
            
            return host;
        }
    }
}