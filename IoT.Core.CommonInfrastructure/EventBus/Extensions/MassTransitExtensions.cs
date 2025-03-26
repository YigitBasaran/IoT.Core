using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace IoT.Core.CommonInfrastructure.EventBus.Extensions
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(
            this IServiceCollection services,
            string hostName,
            string username = "guest",
            string password = "guest")
        {
            services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(hostName, "/", h =>
                    {
                        h.Username(username);
                        h.Password(password);
                    });

                    // 🔹 Register Consumers Dynamically
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}