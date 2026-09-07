using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Contracts;
using ProductApi.Models.AI;

namespace ProductApi.Controllers;

[Route("api/ai")]
[ApiController]
[Authorize]
[ApiVersion("1.0")]
public class AIController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly ILogger<AIController> _logger;

    public AIController(
        IAIService aiService,
        ILogger<AIController> logger)
    {
        _aiService = aiService;
        _logger = logger;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] AIAskRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Question is required"
                });
            }

            var result = await _aiService.AskAsync(request);

            return Ok(new
            {
                Success = true,
                Message = "AI response generated successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing AI request");

            return StatusCode(500, new
            {
                Success = false,
                Message = "Error processing AI request",
                Error = ex.Message
            });
        }
    }
}