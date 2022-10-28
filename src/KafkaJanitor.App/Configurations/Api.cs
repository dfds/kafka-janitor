using System.Text.Json;
using System.Text.Json.Serialization;

namespace KafkaJanitor.App.Configurations;

public static class Api
{
    public static void ConfigureApi(this WebApplicationBuilder builder)
    {
        //builder.Services.AddTransient<Utils.ForwardedPrefix.ForwardedPrefixMiddleware>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services
            .AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });
    }
}