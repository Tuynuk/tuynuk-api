using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;
using Tuynuk.Infrastructure.Models;
using Tuynuk.Infrastructure.Models.Base;

namespace Tuynuk.Api.Data
{
    public class TuynukDbContext : DbContext
    {
        public TuynukDbContext(DbContextOptions<TuynukDbContext> options) : base(options) { }

        public DbSet<Session> Sessions { get; set; }
        public DbSet<Infrastructure.Models.File> Files { get; set; }
        public DbSet<Client> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
               .Property(e => e.Type)
               .HasConversion<string>();

            modelBuilder.Entity<Session>()
                .HasIndex(u => u.Identifier)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }


        private void ConfigurePersistentEntity(ModelBuilder modelBuilder)
        {
            var configureMethod = typeof(TuynukDbContext).GetTypeInfo().DeclaredMethods
                .First(m => m.Name == nameof(ConfigurePersistentEntity) && m.IsGenericMethod);

            foreach (IMutableEntityType mutableEntityType in modelBuilder.Model.GetEntityTypes())
            {
                if (mutableEntityType.ClrType.IsAssignableTo(typeof(PersistentEntity)))
                {
                    configureMethod.MakeGenericMethod(mutableEntityType.ClrType).Invoke(this, new object[] { modelBuilder });
                }
            }
        }

        private void ConfigurePersistentEntity<TEntity>(ModelBuilder modelBuilder) where TEntity : PersistentEntity
        {
            modelBuilder.Entity<TEntity>(builder =>
            {
                builder.HasQueryFilter(f => f.State != Infrastructure.Enums.Base.EntityState.Deleted);
            });

            modelBuilder.Entity<TEntity>()
                .Property(e => e.State)
                .HasConversion<string>();
        }
    }
}
