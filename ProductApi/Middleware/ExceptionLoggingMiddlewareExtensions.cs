using Microsoft.AspNetCore.Builder;
namespace ProductApi.Middleware
{
    public static class ExceptionLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionLoggingMiddleware>();
        }
    }
}
