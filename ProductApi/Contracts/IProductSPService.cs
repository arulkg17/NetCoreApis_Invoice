using ProductApi.DTOs;

namespace ProductApi.Contracts;

public interface IProductSPService
{
    Task<int> CreateAsync(ProductDto dto);
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(ProductDto dto);
    Task<bool> DeleteAsync(int id, string rowVersion);
}
