using System.Diagnostics;
using Microsoft.SemanticKernel;

// Register this filter on the Kernel (e.g. in Program.cs after building it):
//
//   kernel.FunctionInvocationFilters.Add(new FunctionLoggingFilter(logger));
//
// It logs the name of every plugin function the model decides to call,
// the arguments it extracted, and how long each call took. This will
// show you immediately if the same function fires twice, or if an
// unexpected function is being chosen.

public class FunctionLoggingFilter : IFunctionInvocationFilter
{
    private readonly ILogger<FunctionLoggingFilter> _logger;

    public FunctionLoggingFilter(ILogger<FunctionLoggingFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnFunctionInvocationAsync(
        FunctionInvocationContext context,
        Func<FunctionInvocationContext, Task> next)
    {
        var sw = Stopwatch.StartNew();
        var args = string.Join(", ", context.Arguments.Select(a => $"{a.Key}={a.Value}"));

        _logger.LogInformation(
            "KernelFunction CALLED: {Plugin}.{Function}({Args})",
            context.Function.PluginName, context.Function.Name, args);

        await next(context);

        sw.Stop();

        _logger.LogInformation(
            "KernelFunction DONE: {Plugin}.{Function} took {Ms}ms, result={Result}",
            context.Function.PluginName, context.Function.Name, sw.ElapsedMilliseconds,
            context.Result?.ToString());
    }
}
