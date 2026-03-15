
namespace ProductApi.Middleware;

public class CorrelationIDLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIDLoggingMiddleware> _logger;
    private const string CorrelationIDHeaderName = "X-Correlation-ID";

    public CorrelationIDLoggingMiddleware(RequestDelegate next, ILogger<CorrelationIDLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(CorrelationIDHeaderName, out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
            context.Response.Headers[CorrelationIDHeaderName] = correlationId;
        }

        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey(CorrelationIDHeaderName))
            {
                context.Response.Headers[CorrelationIDHeaderName] = correlationId.ToString();
            }
            return Task.CompletedTask;
        });
        // Log correlation id with request path
        _logger.LogInformation("CorrelationID: {CorrelationId} - Request Path: {RequestPath}",
            correlationId, context.Request.Path);

        await _next(context);
    }
}
