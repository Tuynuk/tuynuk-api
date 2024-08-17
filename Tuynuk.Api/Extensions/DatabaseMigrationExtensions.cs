using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;

namespace Tuynuk.Api.Extensions
{
    public static class DatabaseMigrationExtensions
    {
        public static void MigrateDatabase<TDbContext>(this IServiceProvider serviceProvider) where TDbContext : DbContext
        {
            using IServiceScope serviceScope = serviceProvider.CreateScope();
            MigrateDatabase<TDbContext>(serviceScope);
        }

        private static void MigrateDatabase<TDbContext>(IServiceScope serviceScope) where TDbContext : DbContext
        {
            try
            {
                TDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<TDbContext>();
                IConfiguration configuration = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();

                bool doesDatabaseExist = dbContext.Database.GetService<IRelationalDatabaseCreator>().Exists();
                if (!doesDatabaseExist)
                {
                    Log.Information("Database is creating...");
                    dbContext.Database.Migrate();
                    Log.Information("Database is created.");
                }
                else if (dbContext.Database.GetPendingMigrations().Any())
                {
                    Log.Information("Migrating Database...");
                    dbContext.Database.Migrate();
                    Log.Information("Database is migrated.");
                }
                else
                {
                    Log.Information("Database already exists and is up-to-date.");
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(string.Format("Database creation or migration failed: {0}", ex.Message), ex);
                throw;
            }
        }

    }
}
