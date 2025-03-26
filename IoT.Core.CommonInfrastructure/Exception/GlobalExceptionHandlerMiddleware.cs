using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace IoT.Core.CommonInfrastructure.Exception
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BaseException ex)
            {
                await HandleBaseExceptionAsync(context, ex);
            }
            catch (System.Exception ex)
            {
               
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized ||
                    context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    return;
                }

                await HandleGenericExceptionAsync(context, ex);
            }
        }

        private static Task HandleBaseExceptionAsync(HttpContext context, BaseException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception.StatusCode;

            var response = new
            {
                message = exception.Message,
                statusCode = exception.StatusCode
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private static Task HandleGenericExceptionAsync(HttpContext context, System.Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                message = "An unexpected error occurred.",
                statusCode = context.Response.StatusCode
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
