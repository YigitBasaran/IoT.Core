using IoT.Core.CommonInfrastructure.Exception;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace IoT.Core.CommonInfrastructure.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void UseGlobalMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();
    }

    public static void UseSwaggerIfDev(this IApplicationBuilder app, IWebHostEnvironment env, string endpoint, string name)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(endpoint, name);
            });
        }
    }
}