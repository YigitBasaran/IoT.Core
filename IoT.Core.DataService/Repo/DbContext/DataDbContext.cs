using System.Collections.Generic;
using System.Reflection.Emit;
using IoT.Core.DataService.Model;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IoT.Core.DataService.Repo.DbContext
{
    public class DataDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options) { }
        public DbSet<Data> Data { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Data>()
            .Property(c => c.Id)
                .ValueGeneratedNever();
        }
    }
}
