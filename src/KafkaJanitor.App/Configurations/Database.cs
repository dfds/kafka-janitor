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
            options.UseNpgsql(connectionString);

            if (builder.Environment.IsDevelopment())
            {
                options.EnableDetailedErrors();
            }
        });

        //builder.Services.AddTransient<IDbTransactionFacade, RealDbTransactionFacade>();
    }
}