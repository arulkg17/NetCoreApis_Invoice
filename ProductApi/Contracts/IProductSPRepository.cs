using ProductApi.Entities;
namespace ProductApi.Contracts;

public interface IProductSPRepository
{
    Task<int> AddAsync(ProductEntity product);
    Task<IEnumerable<ProductEntity>> GetAllAsync();
    Task<ProductEntity?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(ProductEntity product);
    Task<bool> DeleteAsync(int id, byte[] rowVersion);
}
