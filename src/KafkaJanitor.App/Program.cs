using KafkaJanitor.App.Configurations;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.ConfigureApi();
builder.ConfigureSerilog();
builder.ConfigureSwagger();
builder.ConfigureDatabase();
builder.ConfigureDomain();
builder.ConfigureDomainEvents();

// **PLEASE NOTE** : keep this as the last configuration!
builder.ConfigureAspects(); 

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseSwagger();
app.UseRouting();

app.UseAuthorization();
app.UseHttpMetrics();

app.MapControllers();

app.Run();

#pragma warning disable CA1050 // Declare types in namespaces
public partial class Program
{
}
#pragma warning restore CA1050 // Declare types in namespaces

