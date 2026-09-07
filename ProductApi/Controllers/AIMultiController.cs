using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ProductApi.Contracts;
using ProductApi.Models.AI;
using ProductApi.Repositories;
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

        _logger.LogInformation("AskMulti processing question: {Question}", request.Question);

        var settings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        var systemPrompt =
            """
            You are a helpful assistant for an Invoice Management System.
            Use the available functions to answer the user's question(s).
            If the user asks more than one question in a single message,
            call whichever functions are needed for EACH part, then combine
            everything into one clear, natural-language answer covering all parts.
            If a function returns -1 for an item count, it means the category
            name wasn't found - tell the user that category doesn't exist
            instead of reporting a count.
            """;

        try
        {
            var result = await _kernel.InvokePromptAsync(
                $"{systemPrompt}\n\nUser question: {request.Question}",
                new KernelArguments(settings));

            return Ok(new
            {
                success = true,
                message = "AI response generated successfully",
                data = new { answer = result.ToString() }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing AskMulti request");
            return StatusCode(500, new
            {
                success = false,
                message = "Error processing AI request",
                error = ex.Message
            });
        }
    }
}
