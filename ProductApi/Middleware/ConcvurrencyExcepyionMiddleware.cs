using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ProductApi.Middleware;

public class ConcvurrencyExcepyionMiddleware
{
    private readonly RequestDelegate _next;
    private ILogger<ConcurrencyCheckAttribute> _logger;

    public ConcvurrencyExcepyionMiddleware(
        RequestDelegate next, 
        ILogger<ConcurrencyCheckAttribute> logger) 
        {
            _next = next;
        _logger = logger;
       }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
           await _next(context);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict occurred.");
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            context.Response.ContentType = "application/json";

            var response = new
            {
                message = "The record you attempted for update or delete was modified by another user",
                code = "CONCURRENCY_CONFLICT"
            };
            await context.Response.WriteAsJsonAsync(response);
        }
    }

}
