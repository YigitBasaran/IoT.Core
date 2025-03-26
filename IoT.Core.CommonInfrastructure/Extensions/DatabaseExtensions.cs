using IoT.Core.CommonInfrastructure.Database.Repo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Runtime;
using IoT.Core.CommonInfrastructure.Extensions.DbSettings;

namespace IoT.Core.CommonInfrastructure.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddPostgresDbContext<TDbContext, TEntity, TId, TRepo>(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = "DbSettings")
            where TDbContext : DbContext
            where TEntity : BaseEntity<TId>
            where TRepo : class, IRepository<TEntity, TId>
        {
            var dbSettings = configuration.GetSection(sectionName).Get<PostgreDbSettings>();
            if (dbSettings == null || string.IsNullOrEmpty(dbSettings.ConnectionString))
                throw new InvalidOperationException("EF Core Database connection string is missing in appsettings.json.");

            services.AddSingleton(dbSettings);

            services.AddPooledDbContextFactory<TDbContext>(options =>
                options.UseNpgsql(dbSettings.ConnectionString).EnableSensitiveDataLogging());

            services.AddScoped<TDbContext>(provider =>
            {
                var factory = provider.GetRequiredService<IDbContextFactory<TDbContext>>();
                return factory.CreateDbContext();
            });

            services.AddScoped<IRepository<TEntity, TId>, TRepo>();

            return services;
        }

        public static IServiceCollection AddMongoDatabase<TEntity, TId, TRepo>(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = "DbSettings")
            where TEntity : BaseEntity<TId>
            where TRepo : class, IRepository<TEntity, TId>
        {
            var dbSettings = configuration.GetSection(sectionName).Get<MongoDbSettings>();
            if (dbSettings == null || string.IsNullOrEmpty(dbSettings.ConnectionString))
                throw new InvalidOperationException("MongoDB connection string is missing in appsettings.json.");

            services.AddSingleton(dbSettings);

            services.AddSingleton<IMongoClient>(_ =>
                new MongoClient(dbSettings.ConnectionString));

            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(dbSettings.DatabaseName);
            });

            services.AddScoped<IRepository<TEntity, TId>, TRepo>();

            return services;
        }
    }
}
