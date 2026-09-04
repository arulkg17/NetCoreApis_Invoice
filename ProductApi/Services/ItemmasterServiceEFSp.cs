using AutoMapper;
using ProductApi.Contracts;
using ProductApi.DTOs;
using ProductApi.Entities;

namespace ProductApi.Services;

public class ItemmasterServiceEFSp : IItemmasterService
{
    private readonly IItemmasterRepository _repository;
    private readonly IMapper _mapper;

    public ItemmasterServiceEFSp(
        IItemmasterRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<int> AddAsync(ItemmasterDto dto)
    {
        var entity = _mapper.Map<ItemmasterEntity>(dto);

        return await _repository.AddAsync(entity);
    }

    public async Task<IEnumerable<ItemmasterDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();

        return _mapper.Map<IEnumerable<ItemmasterDto>>(items);
    }

    public async Task<ItemmasterDto?> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);

        return item == null
            ? null
            : _mapper.Map<ItemmasterDto>(item);
    }

    public async Task<bool> UpdateAsync(ItemmasterDto dto)
    {
        var entity = _mapper.Map<ItemmasterEntity>(dto);

        return await _repository.UpdateAsync(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<PagedResultDto<ItemmasterDto>> GetAllPagedAsync(
        ItemmasterFilterDto search)
    {
        var result = await _repository.GetAllPagedAsync(search);

        return new PagedResultDto<ItemmasterDto>
        {
            Data = _mapper.Map<IEnumerable<ItemmasterDto>>(
                result.Data),

            TotalRecords = result.TotalRecords
        };
    }
    public async Task<int> GetActiveItemCountByCategoryAsync(int categoryId)
    {
        var items = await _repository.GetAllAsync();

        return items.Count(x =>
            x.CategoryId == categoryId &&
            x.IsActive == true);
    }
}