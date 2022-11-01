using System;
using System.Threading.Tasks;
using KafkaJanitor.App.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace KafkaJanitor.Tests.Infrastructure;

public class InMemoryDatabaseFactory : IDisposable, IAsyncDisposable
{
    private SqliteConnection? _connection;
    private KafkaJanitorDbContext? _dbContext;

    public async Task<KafkaJanitorDbContext> CreateDbContext(bool initializeSchema = true)
    {
        _connection = new SqliteConnection("Filename=:memory:");
        await _connection.OpenAsync();

        var options = new DbContextOptionsBuilder<KafkaJanitorDbContext>().UseSqlite(_connection).Options;
        _dbContext = new KafkaJanitorDbContext(options);

        if (initializeSchema)
        {
            await _dbContext.Database.EnsureCreatedAsync();
        }

        return _dbContext;
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
        _connection?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_dbContext is not null)
        {
            await _dbContext.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }
}