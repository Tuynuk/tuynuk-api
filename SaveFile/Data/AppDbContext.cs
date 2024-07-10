using Microsoft.EntityFrameworkCore;
using SaveFile.Models;
using File = SaveFile.Models.File;

namespace SaveFile.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Session> Sessions { get; set; }
        public DbSet<File> Files { get; set; }
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
    }
}
