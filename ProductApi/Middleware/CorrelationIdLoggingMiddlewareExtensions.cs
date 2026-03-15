using Microsoft.AspNetCore.Builder;
namespace ProductApi.Middleware
{
    public static class CorrelationIdLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorrelationIdLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorrelationIDLoggingMiddleware >();
        }
    }
}
