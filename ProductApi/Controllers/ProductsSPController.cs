using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Contracts;
using ProductApi.DTOs;

namespace ProductApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsSPController : ControllerBase
{
    private readonly IProductSPService _service;

    public ProductsSPController(IProductSPService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var product = await _service.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductDto dto)
    {
        var id = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProductDto dto)
    {
        if (id != dto.Id)
            return BadRequest();

        var updated = await _service.UpdateAsync(dto);

        if (!updated)
            return Conflict(new
            {
                message = "The record you attempted to edit "
                        + "was modified by another user. Reload and try again."
            });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, [FromQuery] string rowVersion)
    {
        if (string.IsNullOrEmpty(rowVersion))
            return BadRequest("RowVersion is required.");

        var deleted = await _service.DeleteAsync(id, rowVersion);

        if (!deleted)
            return Conflict(new
            {
                message = "Delete failed. The record was modified or already deleted by another user."
            });

        return NoContent();
    }
}
