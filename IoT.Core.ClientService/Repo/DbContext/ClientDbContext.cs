using IoT.Core.ClientService.Model;
using Microsoft.EntityFrameworkCore;

namespace IoT.Core.ClientService.Repo.DbContext
{
    public class ClientDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ClientDbContext(DbContextOptions<ClientDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Client>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // Seed data
            modelBuilder.Entity<Client>().HasData(
                new Client("Tesla", "contact@tesla.com") { Id = 1, CreatedAt = new DateTime(2024, 3, 20, 12, 0, 0, DateTimeKind.Utc) },
                new Client("Google", "info@google.com") { Id = 2, CreatedAt = new DateTime(2024, 3, 20, 12, 0, 0, DateTimeKind.Utc) },
                new Client("Amazon", "support@amazon.com") { Id = 3, CreatedAt = new DateTime(2024, 3, 20, 12, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
