using ProductApi.DTOs;
using ProductApi.Entities;

namespace ProductApi.Contracts;

public interface IItemmasterRepository
{
    Task<int> AddAsync(Itemmaster itemmaster);
    Task<IEnumerable<Itemmaster>> GetAllAsync();
    Task<Itemmaster?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(Itemmaster itemmaster);
    Task<bool> DeleteAsync(int id);
    Task<PagedResultDto<Itemmaster>> GetAllPagedAsync(
        string? catCode,
        string? itemName,
        string? uom,
        int pageNumber,
        int pageSize);
}
