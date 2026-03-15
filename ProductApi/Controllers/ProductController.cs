using Microsoft.AspNetCore.Mvc;
using ProductApi.Contracts;
using ProductApi.DTOs;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Caching.Memory;

namespace ProductApi.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;
        
    public ProductController(IProductService productService, ILogger<ProductController> logger, IConfiguration configuration, IMemoryCache cache)
    {
        _productService = productService;
        _logger = logger;
        _configuration = configuration;
        _cache = cache;
    }

    //[AllowAnonymous]
    //[HttpPost("Login")]
    //public async Task<IActionResult> Login([FromBody] LoginDto dto)
    //{
       
    //    if (!ModelState.IsValid)
    //        return BadRequest();
    //    HttpContext.Session.SetString("MyTest", "My set value test");

    //    var configUsername = _configuration["UserCredentials:Username"];
    //    var configPassword = _configuration["UserCredentials:Password"];

    //    if (dto.UserName != configUsername || dto.Password != configPassword)
    //        return Unauthorized();
    //    var jwtKey = _configuration["Jwt:Key"]; // "v2UJQxTrwUCqqJkehkxvSUZKQCX6gNmRWq7q1bWa3Jw="; // Use config in real app
    //    var jwtIssuer = _configuration["Jwt:Issuer"]; // "yourapiissuer";

    //    var claims = new[]
    //    {
    //        new Claim(ClaimTypes.Name, dto.UserName)
    //    };
    //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    //    var cre = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //    var token = new JwtSecurityToken(
    //        issuer: jwtIssuer,
    //        audience: null,
    //        claims: claims,
    //        expires: DateTime.Now.AddHours(2),
    //        signingCredentials: cre
    //        );
    //    return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });

    //}
    [HttpGet("GetProducts")]
    public async Task<IActionResult> GetProducts()
    {
        //var sessionVal = HttpContext.Session.GetString("MyTest");
        //const string cacheKey = "GetProductsCache";
        //if (_cache.TryGetValue(cacheKey, out List<ProductDto> getProducts))
        //{
        //    _logger.LogInformation("Retrieve product from Cache");
        //    return Ok(getProducts);
        //}

        _logger.LogInformation("Fetching products from database.");
        var products = await _productService.GetAllAsync();

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

        //_cache.Set(cacheKey, products, cacheOptions);
        return Ok(products);

    }
    [HttpGet("GetProduct/{id}")]
    public async Task<IActionResult> GetProduct(int id)
    { 
        _logger.LogInformation($" Fetching Product by Id : {id}",id);
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            _logger.LogInformation($"Product not found : {id}", id);
            return NotFound();
        }

        return Ok(product);
    }
    [HttpPost("CreateProduct")]
    public async Task<IActionResult> CreateProduct(ProductDto dto)
    {
        _logger.LogInformation("Creating Product");
        var created = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetProducts), new { Id = created.Id, Version = "1.0" }, created);
    }
    [HttpPut("UpdateProduct/{id}")]
    public async Task<IActionResult> UpdateProduct(int id, ProductDto dto)
    {
        if (id != dto.Id)
        {
            _logger.LogWarning("ID missmatch in update request");
            return BadRequest("Product ID missmatch");
        }
        var result = await _productService.UpdateAsync(dto);
        if (!result)
        {
            _logger.LogError("Failed to update product id : {id}", id);
            return NotFound();
        }
        return NoContent();
    }
    [HttpDelete("DeleteProduct/{id}")]
    //[SwaggerOperation(Summary="DeleteProduct/Id")]
    public async Task<IActionResult> DeleteProduct(int id)
    { 
        var result = await _productService.DeleteAsync(id);
        if (!result)
        {
            _logger.LogError("Failed to delete product id : {id}",id);
            return NotFound();
        }
        return NoContent();
    }

   
}
