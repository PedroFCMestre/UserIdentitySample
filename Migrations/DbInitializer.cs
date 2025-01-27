using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace Migrations;
public class DbInitializer<T>(
    IServiceProvider serviceProvider,
    IHostEnvironment hostEnvironment,
    IHostApplicationLifetime hostApplicationLifetime
    ) : BackgroundService where T : DbContext
{
    private readonly ActivitySource _activitySource = new(hostEnvironment.ApplicationName);
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = _activitySource.StartActivity(hostEnvironment.ApplicationName, ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<T>();

            EnsureDatabaseAsync(dbContext);
            RunMigrationAsync(dbContext);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();

        return Task.CompletedTask;
    }

    private static void EnsureDatabaseAsync(T dbContext)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        strategy.Execute(() =>
        {
            // Create the database if it does not exist.
            // Do this first so there is then a database to start a transaction against.
            if (!dbCreator.Exists())
            {
                dbCreator.Create();
            }
        });
    }

    private static void RunMigrationAsync(T dbContext)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        strategy.Execute(() =>
        {
            // Run migration in a transaction to avoid partial migration if it fails.
            using var transaction = dbContext.Database.BeginTransaction();
            dbContext.Database.Migrate();
            transaction.Commit();
        });
    }
}
