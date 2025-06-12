using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace projectManagement.Infrastructure.Middleware
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomMiddleware> _logger;

        public CustomMiddleware(RequestDelegate next, ILogger<CustomMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            try
            {
                var request = context.Request;
                var response = context.Response;
                var stream = response.Body;
                await _next(context);

                Console.WriteLine("----------------------------------------");
                Console.WriteLine($"Request path: {request.Path}");
                Console.WriteLine($"Request Method: {request.Method}");
                Console.WriteLine($"Request content type: {context.Request.Headers["Accept"]}");
                Console.WriteLine($"Response type: {response.ContentType}");
                Console.WriteLine($"Response StatusCode: {context.Response.StatusCode}");
                Console.WriteLine($"Response length: {response.ContentLength ?? 0}");
                Console.WriteLine("----------------------------------------");

            }
            catch (Exception ex)
            {
                // Handle and log the exception
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var errorResponse = new
            {
                message = "An unexpected error occurred.",
                details = exception.Message // Avoid exposing sensitive details in production
            };
            return response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}
