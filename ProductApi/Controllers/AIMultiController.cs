using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ProductApi.Models.AI;
namespace ProductApi.Controllers;

[ApiController]
[Route("api/ai")]
//[Authorize]
[ApiVersion("1.0")]
public class AIMultiController : ControllerBase
{
    private readonly Kernel _kernel;
    private readonly ILogger<AIMultiController> _logger;

    public AIMultiController(Kernel kernel, ILogger<AIMultiController> logger)
    {
        _kernel = kernel;
        _logger = logger;
    }

    // New endpoint - supports single OR multiple questions in one request,
    // across Category/Item/Customer/Vendor/User, via real function calling.
    // Existing /api/ai/ask (AIController) is untouched.
    [HttpPost("ask-multi")]
    public async Task<IActionResult> AskMulti([FromBody] AIAskRequest request)
    {
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
