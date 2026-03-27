using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Contracts;
using ProductApi.DTOs;
using ProductApi.Models;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemmasterController : ControllerBase
    {
        private readonly IItemmasterService _service;

        public ItemmasterController(IItemmasterService service)
        {
            _service = service;
        }

        //[HttpGet("GetAllPaged")]
        //public async Task<IActionResult> GetAllPaged(
        //                string? catCode,
        //                string? itemName,
        //                string? uom,
        //                int pageNumber = 1,
        //                int pageSize = 10)
        //{
        //    var query = _context.ItemMasters.AsQueryable();

        //    if (!string.IsNullOrEmpty(catCode))
        //        query = query.Where(x => x.CatCode.Contains(catCode));

        //    if (!string.IsNullOrEmpty(itemName))
        //        query = query.Where(x => x.ItemName.Contains(itemName));

        //    if (!string.IsNullOrEmpty(uom))
        //        query = query.Where(x => x.Uom.Contains(uom));

        //    var totalRecords = await query.CountAsync();

        //    var data = await query
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    return Ok(new
        //    {
        //        data,
        //        totalRecords
        //    });
        //}
        [HttpGet("GetAllPaged")]
        public async Task<IActionResult> GetAllPaged(
                string? catCode,
                string? itemName,
                string? uom,
                int pageNumber = 1,
                int pageSize = 10)
        {
            try
            {
                var result = await _service.GetAllPagedAsync(
                    catCode, itemName, uom, pageNumber, pageSize);

                return Ok(new ApiResponse<PagedResultDto<ItemmasterDto>>
                {
                    Success = true,
                    Message = "Items retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Error retrieving items",
                    Error = new ApiError
                    {
                        Code = "500",
                        Details = ex.Message
                    }
                });
            }
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _service.GetAllAsync();
                return Ok(new ApiResponse<IEnumerable<ItemmasterDto>>
                {
                    Success = true,
                    Message = "Itemmastyer retrieved successfully",
                    Data = data
                });


            }
            catch (Exception ex)
            {

                return StatusCode(500, new ApiResponse<string>{ 
                Success=false,
                Message="Error retrieving Itemmaster",
                        Error = new ApiError { 
                            Code="500",
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
                        Message = "Item not found"
                    });
                }

                return Ok(new ApiResponse<ItemmasterDto>
                {
                    Success = true,
                    Message = "Item retrieved successfully",
                    Data = item
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Error retrieving item",
                    Error = new ApiError
                    {
                        Code = "500",
                        Details = ex.Message
                    }
                });
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(ItemmasterDto dto)
        {
            try
            {
                var id = await _service.AddAsync(dto);

                return Ok(new ApiResponse<int>
                {
                    Success = true,
                    Message = "Item created successfully",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Error creating item",
                    Error = new ApiError
                    {
                        Code = "500",
                        Details = ex.Message
                    }
                });
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(ItemmasterDto dto, int id)
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
                        Message = "Item not found"
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Item updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Error updating item",
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
                        Message = "Item not found"
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Item deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "Error deleting item",
                    Error = new ApiError
                    {
                        Code = "500",
                        Details = ex.Message
                    }
                });
            }
        }
    }
}
