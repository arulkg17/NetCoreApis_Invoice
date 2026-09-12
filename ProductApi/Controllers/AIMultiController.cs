using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ProductApi.Contracts;
using ProductApi.Models.AI;
using ProductApi.Repositories;
using Microsoft.SemanticKernel.ChatCompletion;
namespace ProductApi.Controllers;

[ApiController]
[Route("api/ai")]
//[Authorize]
[ApiVersion("1.0")]
public class AIMultiController : ControllerBase
{
    private readonly Kernel _kernel;
    private readonly IMultiIntentAIProvider _multiIntentAIProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AIMultiController> _logger;
    private readonly ICustomerRepository _customerRepository;
    private readonly IVendorRepository _vendorRepository;
    public AIMultiController(
    Kernel kernel,
    IMultiIntentAIProvider multiIntentAIProvider,
    IConfiguration configuration,
    ILogger<AIMultiController> logger,
    ICustomerRepository customerRepository,
    IVendorRepository vendorRepository)
    {
        _kernel = kernel;
        _multiIntentAIProvider = multiIntentAIProvider;
        _configuration = configuration;
        _logger = logger;
        _customerRepository = customerRepository;
        _vendorRepository = vendorRepository;
    }

    // New endpoint - supports single OR multiple questions in one request,
    // across Category/Item/Customer/Vendor/User, via real function calling.
    // Existing /api/ai/ask (AIController) is untouched.
    [HttpPost("ask-multi")]
    public async Task<IActionResult> AskMulti([FromBody] AIAskRequest request)
    {
        var useMockMultiIntentAI =
    _configuration.GetValue<bool>("UseMockMultiIntentAI");

        if (useMockMultiIntentAI)
        {
            _logger.LogInformation(
                "Using Mock Multi-Intent AI Provider");

            var mockResult =
                await _multiIntentAIProvider.GetMultiIntentAsync(
                    request.Question);

            var intents =
    System.Text.Json.JsonSerializer.Deserialize<
        List<MultiIntentResult>>(
            mockResult,
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (intents == null || intents.Count == 0)
            {
                return Ok(new
                {
                    success = true,
                    message = "AI response generated successfully",
                    data = new
                    {
                        answer = "I could not understand the question."
                    }
                });
            }

            var answers = new List<string>();

            foreach (var intent in intents)
            {
                if (string.Equals(
                        intent.Intent,
                        "CustomerCount",
                        StringComparison.OrdinalIgnoreCase))
                {
                    var count =
                        await _customerRepository
                            .GetCustomerCountAsync(
                                intent.CustomerActiveOnly);

                    var description =
                        intent.CustomerActiveOnly == true
                            ? "active customers"
                            : intent.CustomerActiveOnly == false
                                ? "inactive customers"
                                : "customers";

                    answers.Add(
                        $"There are {count} {description}.");
                }

                else if (string.Equals(
                    intent.Intent,
                    "VendorCount",
                    StringComparison.OrdinalIgnoreCase))
                {
                    var count =
                        await _vendorRepository
                            .GetVendorCountAsync(
                                intent.VendorActiveOnly);

                    var description =
                        intent.VendorActiveOnly == true
                            ? "active vendors"
                            : intent.VendorActiveOnly == false
                                ? "inactive vendors"
                                : "vendors";

                    answers.Add(
                        $"There are {count} {description}.");
                }
            }

            if (answers.Count == 0)
            {
                return Ok(new
                {
                    success = true,
                    message = "AI response generated successfully",
                    data = new
                    {
                        answer =
                            "I don't currently support that type of business question."
                    }
                });
            }

            return Ok(new
            {
                success = true,
                message = "AI response generated successfully",
                data = new
                {
                    answer = string.Join(" ", answers)
                }
            });
        }

        _logger.LogInformation(
     "AskMulti processing question: {Question}",
     request.Question);

        try
        {
            var chatService =
                _kernel.GetRequiredService<IChatCompletionService>();

            var history = new ChatHistory();

            history.AddSystemMessage("""
            You are an AI assistant for an Invoice Management System.

            You have access to business functions through Semantic Kernel plugins.

            IMPORTANT:
            Always use an available business function when the user's question can be answered by one of the functions.
            Do not answer a business-data question from your own knowledge.
            After the function returns, provide a clear and concise natural-language answer.

            ========================
            CATEGORY FUNCTIONS
            ========================

            - If the user asks how many items/products belong to a specific category,
              use CategoryPlugin.get_category_item_count.

            - Example:
              "How many items are in Rice?"
              "How many products are in Snacks?"
              "How many active items are in Rice?"

            ========================
            CUSTOMER FUNCTIONS
            ========================

            - If the user asks for the number of active customers,
              use CustomerPlugin.get_customer_count with isActive=true.

            - If the user asks for the customer count for a specific city,
              use CustomerPlugin.get_customer_count_by_city.

            - If the user asks for customer counts by city,
              all cities, every city, or a citywise breakdown,
              use CustomerPlugin.get_customer_count_all_cities.

            ========================
            VENDOR FUNCTIONS
            ========================

            - If the user asks for the number of active vendors,
              use the appropriate VendorPlugin function.

            - If the user asks for vendor counts or vendor information
              that can be answered by an available VendorPlugin function,
              use that function.

            ========================
            USER FUNCTIONS
            ========================

            - If the user asks for the number of active users,
              use the appropriate UserPlugin function.

            - If the user asks for user information that can be answered
              by an available UserPlugin function,
              use that function.

            ========================
            ITEM MASTER FUNCTIONS
            ========================

            - If the user asks for item/product information that can be answered
              by an available ItemMasterPlugin function,
              use that function.

            - Use ItemMasterPlugin when the question specifically concerns
              item master records.

            ========================
            MULTI-INTENT QUESTIONS
            ========================

            A user may ask more than one business question in the same request.

            For example:

            "How many active customers and active vendors are there?"

            In this case, call the required CustomerPlugin and VendorPlugin functions.

            Another example:

            "How many active customers are there and how many items are in Rice?"

            In this case, call the required CustomerPlugin and CategoryPlugin functions.

            Use all required functions needed to answer every part of the question.

            Do not ignore one part of a multi-intent question.

            ========================
            FINAL RESPONSE
            ========================

            After all required functions have returned their results,
            combine the results into one clear and concise answer.

            Do not mention internal plugin names, function names,
            Semantic Kernel, tool calling, or implementation details
            unless the user explicitly asks about them.
            """);


            history.AddUserMessage(request.Question);

            var settings = new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            _logger.LogInformation(
                "Calling IChatCompletionService for AskMulti...");

            var result = await chatService.GetChatMessageContentAsync(
                history,
                settings,
                _kernel);

            _logger.LogInformation(
                "=== ASK-MULTI SK RESULT: {Result} ===",
                result.Content);

            return Ok(new
            {
                success = true,
                message = "AI response generated successfully",
                data = new
                {
                    answer = result.Content ?? ""
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing AskMulti request");

            return StatusCode(500, new
            {
                success = false,
                message = "Error processing AI request",
                error = ex.Message,
                innerError = ex.InnerException?.Message
            });
        }
    }

    [HttpPost("test-sk-function")]
    public async Task<IActionResult> TestSemanticKernelFunction()
    {
        try
        {
            _logger.LogInformation("=== DIRECT SK FUNCTION TEST START ===");

            var chatService = _kernel.GetRequiredService<IChatCompletionService>();

            var history = new ChatHistory();

            history.AddSystemMessage("""
            You are an assistant for an Invoice Management System.

            When the user asks for the number of active customers,
            use CustomerPlugin.get_customer_count with isActive=true.

            Return the final answer after calling the function.
            """);

            history.AddUserMessage("How many active customers are there?");

            var settings = new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            _logger.LogInformation("Calling IChatCompletionService directly...");

            var result = await chatService.GetChatMessageContentAsync(
                history,
                settings,
                _kernel);

            _logger.LogInformation(
                "=== DIRECT SK RESULT: {Result} ===",
                result.Content);

            return Ok(new
            {
                success = true,
                answer = result.Content
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Direct Semantic Kernel function test failed");

            return StatusCode(500, new
            {
                success = false,
                error = ex.Message,
                innerError = ex.InnerException?.Message
            });
        }
    }

}
