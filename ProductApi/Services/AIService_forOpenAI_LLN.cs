#pragma warning disable OPENAI001
#pragma warning disable SCME0001

using OpenAI.Responses;
using ProductApi.Contracts;
using ProductApi.Models.AI;

namespace ProductApi.Services;

public class AIService_forOpenAI_LLN : IAIService
{
    private readonly ILogger<AIService_forOpenAI_LLN> _logger;
    private readonly OpenAISettings _settings;
    private readonly ResponsesClient _client;

    public AIService_forOpenAI_LLN(
        ILogger<AIService_forOpenAI_LLN> logger,
        IConfiguration configuration)
    {
        _logger = logger;

        _settings = configuration
            .GetSection("OpenAI")
            .Get<OpenAISettings>()
            ?? throw new InvalidOperationException(
                "OpenAI configuration is missing.");

        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            throw new InvalidOperationException(
                "OpenAI API key is missing.");
        }

        _client = new ResponsesClient(_settings.ApiKey);
    }

    public async Task<AIAskResponse> AskAsync(AIAskRequest request)
    {
        _logger.LogInformation(
            "Sending question to OpenAI: {Question}",
            request.Question);

        var options = new CreateResponseOptions
        {
            Model = _settings.Model
        };

        options.InputItems.Add(
            ResponseItem.CreateUserMessageItem(request.Question));

        var result = await _client.CreateResponseAsync(options);

        return new AIAskResponse
        {
            Answer = result.Value.GetOutputText()
        };
    }
}

#pragma warning restore SCME0001
#pragma warning restore OPENAI001