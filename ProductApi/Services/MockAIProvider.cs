using System.Text.Json;
using ProductApi.Contracts;

namespace ProductApi.Services;

public class MockAIProvider : IMockAIProvider
{
    private readonly ILogger<MockAIProvider> _logger;

    public MockAIProvider(
        ILogger<MockAIProvider> logger)
    {
        _logger = logger;
    }

    public Task<string> GetIntentAsync(string question)
    {
        _logger.LogInformation(
            "Mock LLM processing question: {Question}",
            question);

        var normalizedQuestion =
            question.Trim().ToLowerInvariant();

        // ============================================
        // Category Item Count
        // ============================================

        if (normalizedQuestion.Contains("how many") &&
            normalizedQuestion.Contains("item"))
        {
            string? categoryName = null;

            // --------------------------------------------
            // Detect category
            // --------------------------------------------

            if (normalizedQuestion.Contains("rice"))
            {
                categoryName = "Rice";
            }
            else if (normalizedQuestion.Contains("vegitables") ||
                     normalizedQuestion.Contains("vegetables"))
            {
                categoryName = "Vegitables";
            }
            else if (normalizedQuestion.Contains("snacks"))
            {
                categoryName = "Snacks";
            }
            else if (normalizedQuestion.Contains("oil"))
            {
                categoryName = "Oil";
            }
            else if (normalizedQuestion.Contains("soap"))
            {
                categoryName = "Soap";
            }
            else if (normalizedQuestion.Contains("general"))
            {
                categoryName = "General";
            }
            else if (normalizedQuestion.Contains("pulses"))
            {
                categoryName = "Pulses";
            }
            bool? itemActiveOnly;

            if (normalizedQuestion.Contains("inactive"))
            {
                itemActiveOnly = false;
            }
            else if (normalizedQuestion.Contains("active"))
            {
                itemActiveOnly = true;
            }
            else
            {
                itemActiveOnly = null;
            }
            var intent = new
            {
                intent = "CategoryItemCount",
                categoryName = categoryName,
                categoryActiveOnly = true,
                itemActiveOnly = itemActiveOnly
            };

            var json =
                JsonSerializer.Serialize(intent);

            _logger.LogInformation(
                "Mock LLM returned intent: {Intent}",
                json);

            return Task.FromResult(json);
        }

        // ============================================
        // Unsupported question
        // ============================================

        var unsupportedIntent = new
        {
            intent = "Unknown",
            categoryName = (string?)null,
            categoryActiveOnly = (bool?)null,
            itemActiveOnly = (bool?)null
        };

        var unsupportedJson =
            JsonSerializer.Serialize(
                unsupportedIntent);

        _logger.LogInformation(
            "Mock LLM returned unsupported intent: {Intent}",
            unsupportedJson);

        return Task.FromResult(unsupportedJson);
    }
}
