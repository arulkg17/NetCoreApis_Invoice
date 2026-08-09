using ProductApi.DTOs;

namespace ProductApi.Contracts;

public interface IItemmasterService
{
    Task<int> AddAsync(ItemmasterDto dto);

    Task<IEnumerable<ItemmasterDto>> GetAllAsync();

    Task<ItemmasterDto?> GetByIdAsync(int id);

    Task<bool> UpdateAsync(ItemmasterDto dto);

    Task<bool> DeleteAsync(int id);

    Task<PagedResultDto<ItemmasterDto>> GetAllPagedAsync(
        ItemmasterFilterDto search);
}