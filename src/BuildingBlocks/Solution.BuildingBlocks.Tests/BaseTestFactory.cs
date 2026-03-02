using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Solution.API;

namespace Solution.BuildingBlocks.Tests;

public abstract class BaseTestFactory<TDbContext> : WebApplicationFactory<Program> where TDbContext : DbContext
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            using var scope = BuildServiceProvider(services).CreateScope();
            var scopedServices = scope.ServiceProvider;  
            var db = scopedServices.GetRequiredService<TDbContext>();
            var logger = scopedServices.GetRequiredService<ILogger<BaseTestFactory<TDbContext>>>();

            var path = Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "../../../../../../Modules/UniLeague/Solution.UniLeague.Tests/TestData"
            ));
            InitializeDatabase(db, path, logger);
        });
    }

    private static void InitializeDatabase(DbContext context, string scriptFolder, ILogger logger)
    {
        try
        {
            context.Database.EnsureCreated();
            var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }
        catch (Exception ex)
        {
            
        }

        try
        {
            logger.LogInformation("Seeding scripts folder: {Folder}", scriptFolder);

            var scriptFiles = Directory.GetFiles(scriptFolder, "*.sql");
            Array.Sort(scriptFiles);

            logger.LogInformation("Found {Count} SQL scripts: {Files}",
                scriptFiles.Length,
                string.Join(", ", scriptFiles.Select(Path.GetFileName)));

            if (scriptFiles.Length == 0)
                throw new InvalidOperationException($"No .sql scripts found in: {scriptFolder}");


            Array.Sort(scriptFiles);
            foreach (var file in scriptFiles)
            {
                var sql = File.ReadAllText(file);
                context.Database.ExecuteSqlRaw(sql);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Seeding failed: {Message}", ex.Message);
            throw;
        }
    }

    private ServiceProvider BuildServiceProvider(IServiceCollection services)
    {
        return ReplaceNeededDbContexts(services).BuildServiceProvider();
    }

    protected abstract IServiceCollection ReplaceNeededDbContexts(IServiceCollection services);

    protected static Action<DbContextOptionsBuilder> SetupTestContext()
    {
        var server = Environment.GetEnvironmentVariable("DATABASE_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "5432";
        var database = Environment.GetEnvironmentVariable("DATABASE_SCHEMA") ?? "unileaguedb-test";
        var user = Environment.GetEnvironmentVariable("DATABASE_USERNAME") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "root";
        var pooling = Environment.GetEnvironmentVariable("DATABASE_POOLING") ?? "true";

        var connectionString = $"Server={server};Port={port};Database={database};User ID={user};Password={password};Pooling={pooling};Include Error Detail=True";

        Console.WriteLine($"[TEST DB] Database={database} Server={server} Port={port} User={user}");

        return opt => opt.UseNpgsql(connectionString);
    }
}