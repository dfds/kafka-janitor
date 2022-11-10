using KafkaJanitor.App.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KafkaJanitor.App.Configurations;

public static class Database
{
    public static void ConfigureDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<KafkaJanitorDbContext>((serviceProvider, options) =>
        {
            var connectionString = builder.Configuration["DB_CONNECTION_STRING"];

            if (builder.Environment.IsDevelopment())
            {
                options.EnableDetailedErrors();
            }

            options.UseNpgsql(connectionString);
        });

        builder.Services.AddTransient<IDbTransactionFacade, RealDbTransactionFacade>();
    }
} 