using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace IoT.Core.CommonInfrastructure.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, string title, string version, string description)
    {
        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SwaggerDoc(version, new OpenApiInfo
            {
                Title = title,
                Version = version,
                Description = description
            });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter JWT Bearer token",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            c.AddSecurityDefinition("Bearer", securityScheme);

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, new[] { "Bearer" } }
            });
        });

        return services;
    }
}