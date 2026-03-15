using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using ProductApi.Controllers;
using ProductApi.Contracts;
using ProductApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProductApi.Tests
{
    public class ProductControllerTests
    {
        // ARRANGE: Setup dependencies (using Moq)
        private readonly Mock<IProductService> _productServiceMock = new();
        private readonly ILogger<ProductController> _logger = new LoggerFactory().CreateLogger<ProductController>();
        private readonly Mock<IConfiguration> _configMock = new();
        private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        private ProductController CreateController() =>
            new ProductController(_productServiceMock.Object, _logger, _configMock.Object, _cache);

        [Fact]
        public async Task GetProducts_ReturnsOk_WithProducts()
        {
            // Arrange
            var products = new List<ProductDto> { new ProductDto { Id = 1, Name = "Test" } };
            _productServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

            // Act
            var controller = CreateController();
            var result = await controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(products, okResult.Value);
        }

        //[Fact]
        //public async Task GetProduct_ReturnsNotFound_IfMissing()
        //{
        //    // Arrange
        //    _productServiceMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((ProductDto)null);
        //    var controller = CreateController();

        //    // Act
        //    var result = await controller.GetProduct(999);

        //    // Assert
        //    Assert.IsType<NotFoundResult>(result);
        //}
    }
}
