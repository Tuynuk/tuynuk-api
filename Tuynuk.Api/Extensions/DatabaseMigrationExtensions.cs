using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

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
                    Console.WriteLine("Database is creating...");
                    dbContext.Database.Migrate();
                    Console.WriteLine("Database is created.");
                }
                else if (dbContext.Database.GetPendingMigrations().Any())
                {
                    Console.WriteLine("Migrating Database...");
                    dbContext.Database.Migrate();
                    Console.WriteLine("Database is migrated.");
                }
                else
                {
                    Console.WriteLine("Database already exists and is up-to-date.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database creation or migration failed.");
                throw;
            }
        }

    }
}
