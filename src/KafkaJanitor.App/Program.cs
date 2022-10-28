using KafkaJanitor.App.Configurations;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.ConfigureApi();
builder.ConfigureSerilog();
builder.ConfigureSwagger();
builder.ConfigureDomain();
builder.ConfigureDatabase();

var app = builder.Build();

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