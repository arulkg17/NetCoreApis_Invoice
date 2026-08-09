using ProductApi.DTOs;
using ProductApi.Entities;

namespace ProductApi.Contracts;

public interface IItemmasterRepository
{
    Task<int> AddAsync(ItemmasterEntity itemmaster);

    Task<IEnumerable<ItemmasterEntity>> GetAllAsync();

    Task<ItemmasterEntity?> GetByIdAsync(int id);

    Task<bool> UpdateAsync(ItemmasterEntity itemmaster);

    Task<bool> DeleteAsync(int id);

    Task<PagedResultDto<ItemmasterEntity>> GetAllPagedAsync(
        ItemmasterFilterDto search);
}