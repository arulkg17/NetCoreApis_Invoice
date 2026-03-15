using ProductApi.DTOs;

namespace ProductApi.Contracts
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(ProductDto productDto);
        Task<bool> UpdateAsync(ProductDto productDto);
        Task<bool> DeleteAsync(int id);
    }
}
