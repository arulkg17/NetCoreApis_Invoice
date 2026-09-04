using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Contracts;
using ProductApi.DTOs;

namespace ProductApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _service;
    private readonly ILogger<UserController> _logger;

    public UserController(
        IUserService service,
        ILogger<UserController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // ============================================
    // GET ALL
    // ============================================

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var response = await _service.GetAllAsync();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while getting all users.");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    Success = false,
                    Message = "An error occurred while retrieving users."
                });
        }
    }

    // ============================================
    // GET BY ID
    // ============================================

    [HttpGet("GetById/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var response = await _service.GetByIdAsync(id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while getting user. UserId: {UserId}",
                id);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    Success = false,
                    Message = "An error occurred while retrieving the user."
                });
        }
    }

    // ============================================
    // GET PAGED
    // ============================================

    [HttpPost("GetPaged")]
    public async Task<IActionResult> GetPaged(
        [FromBody] UserFilterDto filter)
    {
        try
        {
            var response =
                await _service.GetPagedAsync(filter);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while getting paged users.");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    Success = false,
                    Message = "An error occurred while retrieving users."
                });
        }
    }

    // ============================================
    // CREATE
    // ============================================

    [HttpPost("Create")]
    [AllowAnonymous]
    public async Task<IActionResult> Create(
        [FromBody] UserCreateDto dto)
    {
        try
        {
            var response =
                await _service.CreateAsync(dto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while creating user.");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    Success = false,
                    Message = "An error occurred while creating the user."
                });
        }
    }

    // ============================================
    // UPDATE
    // ============================================

    [HttpPut("Update/{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UserUpdateDto dto)
    {
        try
        {
            var response =
                await _service.UpdateAsync(id, dto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while updating user. UserId: {UserId}",
                id);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    Success = false,
                    Message = "An error occurred while updating the user."
                });
        }
    }

    // ============================================
    // DELETE
    // ============================================

    [HttpDelete("Delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var response =
                await _service.DeleteAsync(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while deleting user. UserId: {UserId}",
                id);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    Success = false,
                    Message = "An error occurred while deleting the user."
                });
        }
    }

}
