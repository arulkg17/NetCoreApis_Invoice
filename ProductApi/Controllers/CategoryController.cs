using ProductApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Contracts;
using ProductApi.DTOs;

namespace ProductApi.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _service;
    private readonly ILogger<CategoryController> _logger;
    public CategoryController(ICategoryService service, ILogger<CategoryController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var data = await _service.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<CategoryDto>>
            {
                Success = true,
                Message = "Categories retrieved successfully",
                Data = data
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>
            {
                Success = false,
                Message = "Error retrieving Category",
                Error = new ApiError
                {
                    Code = "500",
                    Details = ex.Message
                }
            });
        }
    }

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Category not found"
                });
            }
            return Ok(new ApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Category retrieved successfully",
                Data = item
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>
            {
                Success = false,
                Message = "Error retrieving Category",
                Error = new ApiError
                {
                    Code = "500",
                    Details = ex.Message
                }
            });
        }
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(CategoryDto dto)
    {
        try
        {
            var id = await _service.AddAsync(dto);
            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Category created successfully",
                Data = id
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>
            {
                Success = false,
                Message = "Error creating category",
                Error = new ApiError
                {
                    Code = "500",
                    Details = ex.Message
                }
            });
        }
    }

    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(int id, CategoryDto dto)
    {
        try
        {
            dto.Id = id;
            var updated = await _service.UpdateAsync(dto);
            if (!updated)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Category not found"
                });
            }
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Category Updated successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>
            {
                Success = false,
                Message = "Error updating Category",
                Error = new ApiError
                {
                    Code = "500",
                    Details = ex.Message
                }
            });
        }
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Category not found"
                });
            }
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Category deleted successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>
            {
                Success = false,
                Message = "Error deleting Category",
                Error = new ApiError

                {
                    Code = "500",
                    Details = ex.Message
                }
            });
        }
    }
    [HttpGet("GetAllPaged")]
    public async Task<IActionResult> GetAllPaged(
    string? Code,
    string? Name,
    int pageNumber = 1,
    int pageSize = 10)
    {
        try
        {
            var result = await _service.GetAllPagedAsync(
                Code, Name, pageNumber, pageSize);

            return Ok(new ApiResponse<IEnumerable<CategoryDto>>
            {
                Success = true,
                Message = "Category retrieved successfully",
                Data = result.Data,
                TotalRecords = result.TotalRecords
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>
            {
                Success = false,
                Message = "Error retrieving Category",
                Error = new ApiError
                {
                    Code = "500",
                    Details = ex.Message
                }
            });
        }
    }
}