using System.Text.Json;
using ProductApi.Contracts;
using ProductApi.Models.AI;

namespace ProductApi.Services;

public class AIService : IAIService
{
    private readonly ILogger<AIService> _logger;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMockAIProvider _mockAIProvider;

    public AIService(
        ILogger<AIService> logger,
        ICategoryRepository categoryRepository,
        IMockAIProvider mockAIProvider)
    {
        _logger = logger;
        _categoryRepository = categoryRepository;
        _mockAIProvider = mockAIProvider;
    }

    public async Task<AIAskResponse> AskAsync(
        AIAskRequest request)
    {
        _logger.LogInformation(
            "Processing AI question: {Question}",
            request.Question);

        // ============================================
        // STEP 1
        // Mock LLM understands the question
        // ============================================

        var intentJson =
            await _mockAIProvider.GetIntentAsync(
                request.Question);

        _logger.LogInformation(
            "Intent received from Mock LLM: {Intent}",
            intentJson);

        // ============================================
        // STEP 2
        // Convert LLM JSON to C# object
        // ============================================

        var intent =
            JsonSerializer.Deserialize<AIIntent>(
                intentJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (intent == null)
        {
            throw new InvalidOperationException(
                "Unable to understand AI intent.");
        }

        // ============================================
        // STEP 3
        // Check supported intent
        // ============================================

        if (!string.Equals(
                intent.Intent,
                "CategoryItemCount",
                StringComparison.OrdinalIgnoreCase))
        {
            return new AIAskResponse
            {
                Answer =
                    "I don't currently support that type of business question."
            };
        }

        // ============================================
        // STEP 4
        // Validate category
        // ============================================

        if (string.IsNullOrWhiteSpace(
                intent.CategoryName))
        {
            return new AIAskResponse
            {
                Answer =
                    "I could not identify the category from your question."
            };
        }

        // ============================================
        // STEP 5
        // Find category
        // ============================================

        var categories =
            await _categoryRepository.GetAllAsync();

        var requestedCategoryName =
            intent.CategoryName.Trim();

        var category =
            categories.FirstOrDefault(
                x => !string.IsNullOrWhiteSpace(x.Name) &&
                     string.Equals(
                         x.Name.Trim(),
                         requestedCategoryName,
                         StringComparison.OrdinalIgnoreCase));

        if (category == null)
        {
            return new AIAskResponse
            {
                Answer =
                    $"I could not find the category '{intent.CategoryName}'."
            };
        }

        // ============================================
        // STEP 6
        // Validate category status
        // ============================================

        if (intent.CategoryActiveOnly == true &&
            category.IsActive != true)
        {
            return new AIAskResponse
            {
                Answer =
                    $"The category '{category.Name}' is currently inactive."
            };
        }

        // ============================================
        // STEP 7
        // Get actual database data
        // ============================================

        var result =
            await _categoryRepository
                .GetCategoryItemCountAsync(
                    category.Name.Trim(),
                    intent.CategoryActiveOnly ?? false,
                    intent.ItemActiveOnly);

        if (result == null)
        {
            return new AIAskResponse
            {
                Answer =
                    $"I could not retrieve item information for the category '{category.Name}'."
            };
        }

        _logger.LogInformation(
            "Database result - Category: {Category}, ItemActiveOnly: {ItemActiveOnly}, Count: {Count}",
            result.CategoryName,
            intent.ItemActiveOnly,
            result.ItemCount);

        // ============================================
        // STEP 8
        // Build answer
        // ============================================

        var cleanCategoryName =
            category.Name.Trim();

        string itemDescription;

        if (intent.ItemActiveOnly == true)
        {
            itemDescription = "active items";
        }
        else if (intent.ItemActiveOnly == false)
        {
            itemDescription = "inactive items";
        }
        else
        {
            itemDescription = "items";
        }

        var answer =
            $"The {cleanCategoryName} category is active and has {result.ItemCount} {itemDescription}.";

        return new AIAskResponse
        {
            Answer = answer
        };
    }
}