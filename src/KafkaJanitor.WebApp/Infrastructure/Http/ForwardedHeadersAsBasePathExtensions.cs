using Microsoft.AspNetCore.Builder;

namespace KafkaJanitor.WebApp.Infrastructure.Http
{
    public static class ForwardedHeadersAsBasePathExtensions
    {
        public static IApplicationBuilder UseForwardedHeadersAsBasePath(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ForwardedHeaderBasePath>();
        }
    }
}