using System.Text.RegularExpressions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using ProductApi.Contracts;

namespace ProductApi.Services;

// v2: adds a "reason before answering" step (chain-of-thought) to fix
// small models mis-reading negation words like "inactive".
public class OllamaAIProvider : IMockAIProvider
{
    private readonly ILogger<OllamaAIProvider> _logger;
    private readonly ICategoryRepository _categoryRepository;
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatService;

    public OllamaAIProvider(
        ILogger<OllamaAIProvider> logger,
        ICategoryRepository categoryRepository,
        Kernel kernel)
    {
        _logger = logger;
        _categoryRepository = categoryRepository;
        _kernel = kernel;
        _chatService = kernel.GetRequiredService<IChatCompletionService>();
    }

    public async Task<string> GetIntentAsync(string question)
    {
        _logger.LogInformation(
            "Ollama LLM processing question: {Question}",
            question);

        var categories = await _categoryRepository.GetAllAsync();
        var categoryNames = string.Join(
            ", ",
            categories
                .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                .Select(c => c.Name!.Trim()));

        var systemPrompt =
            $$"""
            You are an intent-extraction engine for an Invoice Management System.
            Known categories: {{categoryNames}}.

            Work through this in two steps:

            STEP 1 - Reason briefly (1-2 short lines):
            - Which category is mentioned?
            - Does the question say the exact word "active" or the exact word "inactive"?
              These are DIFFERENT words - check letter by letter which one is present.
              If neither word appears, say "no qualifier".

            STEP 2 - Then output ONLY a raw JSON object on its own line, in exactly this shape,
            with no markdown fences:

            {
              "intent": "CategoryItemCount" or "Unknown",
              "categoryName": "<one of the known categories, or null>",
              "categoryActiveOnly": true,
              "itemActiveOnly": true, false, or null
            }

            Rules:
            - categoryActiveOnly is ALWAYS true for this kind of question - it is not
              related to whether items are active or inactive.
            - itemActiveOnly is true if and only if the word "active" appears
              (and NOT "inactive").
            - itemActiveOnly is false if and only if the word "inactive" appears.
            - itemActiveOnly is null only if neither word appears.
            - Match categoryName to the closest known category, fixing typos/plurals.
            - If unrelated to category item counts, return intent "Unknown", other fields null.

            Examples:
            Q: "How many active items are in Rice?"
            Step 1: Category is Rice. The word "active" appears (not "inactive").
            {"intent":"CategoryItemCount","categoryName":"Rice","categoryActiveOnly":true,"itemActiveOnly":true}

            Q: "How many inactive items are in Rice?"
            Step 1: Category is Rice. The word "inactive" appears.
            {"intent":"CategoryItemCount","categoryName":"Rice","categoryActiveOnly":true,"itemActiveOnly":false}

            Q: "How many items are in Rice?"
            Step 1: Category is Rice. No qualifier word appears.
            {"intent":"CategoryItemCount","categoryName":"Rice","categoryActiveOnly":true,"itemActiveOnly":null}
            """;

        var history = new ChatHistory(systemPrompt);
        history.AddUserMessage(question);

        var response = await _chatService.GetChatMessageContentAsync(
            history,
            kernel: _kernel);

        var fullText = (response.Content ?? "{}").Trim();

        _logger.LogInformation(
            "Ollama LLM full response (reasoning + JSON): {FullText}",
            fullText);

        // The model now outputs reasoning THEN json - extract just the
        // last {...} block in the response, ignoring the reasoning text
        // and any stray ```json fences.
        var cleaned = fullText
            .Replace("```json", "", StringComparison.OrdinalIgnoreCase)
            .Replace("```", "");

        var matches = Regex.Matches(cleaned, @"\{[^{}]*\}");
        var json = matches.Count > 0
            ? matches[matches.Count - 1].Value
            : "{}";

        _logger.LogInformation(
            "Ollama LLM extracted intent JSON: {Intent}",
            json);

        return json;
    }
}