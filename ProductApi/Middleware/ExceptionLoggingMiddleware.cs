using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Text.Json;

namespace ProductApi.Middleware
{
    public class ExceptionLoggingMiddleware
    {
        private readonly Serilog.ILogger _logger;
        private readonly RequestDelegate _next;
        public ExceptionLoggingMiddleware(RequestDelegate next)
        { 
            _logger = Log.ForContext<ExceptionLoggingMiddleware>(); 
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                // Log the exception including stacktrace
                _logger.Error(ex, "Unhandled exception caught by middleware");

                // Prepare generic error response
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var errorResponse = new { message = "An unexpected error occured." };
                var jsonResponse = JsonSerializer.Serialize(errorResponse);

                await context.Response.WriteAsync(jsonResponse);

                // Middleware handled the error; do NOT rethrow
            }
        }
    }
}
