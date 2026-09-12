using System.Diagnostics;

namespace ProductApi.AI;

public sealed class OllamaLoggingHandler : DelegatingHandler
{
    private const string LogFile = "ollama-http-debug.txt";

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        var requestBody = request.Content == null
            ? string.Empty
            : await request.Content.ReadAsStringAsync(cancellationToken);

        var requestLog =
            $"""
            ============================================================
            OLLAMA HTTP REQUEST
            Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}
            Method: {request.Method}
            URL: {request.RequestUri}

            Headers:
            {request.Headers}

            Body:
            {requestBody}
            ============================================================
            """;

        await File.AppendAllTextAsync(
            LogFile,
            requestLog + Environment.NewLine,
            cancellationToken);

        Console.WriteLine(requestLog);

        var response = await base.SendAsync(
            request,
            cancellationToken);

        var responseBody = response.Content == null
            ? string.Empty
            : await response.Content.ReadAsStringAsync(cancellationToken);

        stopwatch.Stop();

        var responseLog =
            $"""
            ============================================================
            OLLAMA HTTP RESPONSE
            Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}
            Status: {(int)response.StatusCode}
            Elapsed: {stopwatch.ElapsedMilliseconds} ms

            Body:
            {responseBody}
            ============================================================
            """;

        await File.AppendAllTextAsync(
            LogFile,
            responseLog + Environment.NewLine,
            cancellationToken);

        Console.WriteLine(responseLog);

        return response;
    }
}