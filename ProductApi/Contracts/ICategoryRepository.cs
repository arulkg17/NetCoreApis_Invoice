using ProductApi.DTOs;
using ProductApi.Entities;

namespace ProductApi.Contracts;

public interface ICategoryRepository
{
    Task<int> AddAsync(CategoryEntity category);
    Task<IEnumerable<CategoryEntity>> GetAllAsync();
    Task<CategoryEntity?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(CategoryEntity category);
    Task<bool> DeleteAsync(int id);
    Task<PagedResultDto<CategoryEntity>> GetAllPagedAsync(
    string? Code,
    string? Name,
    int pageNumber,
    int pageSize);
}
