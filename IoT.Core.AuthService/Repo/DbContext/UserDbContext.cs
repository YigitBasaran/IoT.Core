using IoT.Core.AuthService.Model;
using Microsoft.EntityFrameworkCore;

namespace IoT.Core.AuthService.Repo.DbContext
{
    public class UserDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(c => c.Id)
                .ValueGeneratedNever();
            modelBuilder.Entity<User>()
                .HasIndex(c => c.Username)
                .IsUnique();

            // Seed data

            modelBuilder.Entity<User>().HasData(
                new User("admin", "$2a$11$938geM.vJ0pq5raKUfMBB.y0Wq6MSuIefVoptpY5kFEVI4Palx2WC", RoleEnum.Operator) { Id = 0, CreatedAt = new DateTime(2024, 3, 20, 12, 0, 0, DateTimeKind.Utc) },
                new User("Tesla", "$2a$11$djNSsxIQuzCrU1AQktDX4uqysqnNLaX9OhLDdq5ZUIXW7qWXPC2na", RoleEnum.Client) { Id = 1, CreatedAt = new DateTime(2024, 3, 20, 12, 0, 0, DateTimeKind.Utc) },
                new User("Google", "$2a$11$Gw1V.HcCVJamgzWmKyzO6O/8WoSJrzK5oO9gAthNWTXLt3iaE2GHq", RoleEnum.Client) { Id = 2, CreatedAt = new DateTime(2024, 3, 20, 12, 0, 0, DateTimeKind.Utc) },
                new User("Amazon", "$2a$11$nImEvlk9fjvOLY9Qkqkdm.MAbvaH1li6jpm3J7jl1n5EHHzWa8xSK", RoleEnum.Client) { Id = 3, CreatedAt = new DateTime(2024, 3, 20, 12, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
