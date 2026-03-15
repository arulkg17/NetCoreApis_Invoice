using ProductApi.Contracts;
using ProductApi.DTOs;
using ProductApi.Entities;

namespace ProductApi.Services;

public class ProductSPService: IProductSPService
{
    private readonly IProductSPRepository _repo;

    public ProductSPService(IProductSPRepository repo)
    {
        _repo = repo;
    }

    public async Task<int> CreateAsync(ProductDto dto)
    {
        var product = new ProductEntity
        {
            Name = dto.Name,
            Price = dto.Price,
            Stock = dto.Stock
        };

        return await _repo.AddAsync(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _repo.GetAllAsync();

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Stock = p.Stock,
            RowVersion = Convert.ToBase64String(p.RowVersion)
        });
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null) return null;

        return new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Stock = p.Stock,
            RowVersion = Convert.ToBase64String(p.RowVersion)
        };
    }

    public async Task<bool> UpdateAsync(ProductDto dto)
    {
        var product = new ProductEntity
        {
            Id = dto.Id,
            Name = dto.Name,
            Price = dto.Price,
            Stock = dto.Stock,
            RowVersion = Convert.FromBase64String(dto.RowVersion!)
        };

        return await _repo.UpdateAsync(product);
    }

    public async Task<bool> DeleteAsync(int id, string rowVersion)
    {
        var versionBytes = Convert.FromBase64String(rowVersion);
        return await _repo.DeleteAsync(id, versionBytes);
    }
}
