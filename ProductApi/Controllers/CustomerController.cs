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

namespace ProductApi.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
            
        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger, IConfiguration configuration, IMemoryCache cache)
        {
            _customerService = customerService;
            _logger = logger;
            _configuration = configuration;
            _cache = cache;
        }
               
        [Authorize]
        [HttpGet("GetCustomers")]
        public async Task<IActionResult> GetCustomers()
        {
            //var sessionVal = HttpContext.Session.GetString("MyTest");
            //const string cacheKey = "GetCustomersCache";
            //if (_cache.TryGetValue(cacheKey, out List<CustomerDto> getCustomers))
            //{
            //    _logger.LogInformation("Retrieve customer from Cache");
            //    return Ok(getCustomers);
            //}

            _logger.LogInformation("Fetching customers from database.");
            var products = await _customerService.GetAllAsync();

            //var cacheOptions = new MemoryCacheEntryOptions
            //{
            //    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            //    SlidingExpiration = TimeSpan.FromMinutes(2)
            //};

            //_cache.Set(cacheKey, products, cacheOptions);
            return Ok(products);

        }
        [Authorize]
        [HttpGet("GetCustomer/{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        { 
            _logger.LogInformation($" Fetching Customer by Id : {id}",id);
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
            {
                _logger.LogInformation($"Customer not found : {id}", id);
                return NotFound();
            }

            return Ok(customer);
        }
        [Authorize]
        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer(CustomerDto dto)
        {
            _logger.LogInformation("Creating Customer");
            var created = await _customerService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetCustomers), new { Id = created.Id, Version = "1.0" }, created);
        }
        [Authorize]
        [HttpPut("UpdateCustomer/{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, CustomerDto dto)
        {
            if (id != dto.Id)
            {
                _logger.LogWarning("ID missmatch in update request");
                return BadRequest("Customer ID missmatch");
            }
            var result = await _customerService.UpdateAsync(dto);
            if (!result)
            {
                _logger.LogError("Failed to update customer id : {id}", id);
                return NotFound();
            }
            return NoContent();
        }
        [Authorize]
        [HttpDelete("DeleteCustomer/{id}")]
        //[SwaggerOperation(Summary="DeleteCustomer/Id")]
        public async Task<IActionResult> DeleteCustomer(int id)
        { 
            var result = await _customerService.DeleteAsync(id);
            if (!result)
            {
                _logger.LogError("Failed to delete customer id : {id}",id);
                return NotFound();
            }
            return NoContent();
        }

       
    }
}
