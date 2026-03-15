using ProductApi.DTOs;

namespace ProductApi.Contracts
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllAsync();
        Task<CustomerDto> GetByIdAsync(int id);
        Task<CustomerDto> CreateAsync(CustomerDto CustomerDto);
        Task<bool> UpdateAsync(CustomerDto CustomerDto);
        Task<bool> DeleteAsync(int id);
    }
}
