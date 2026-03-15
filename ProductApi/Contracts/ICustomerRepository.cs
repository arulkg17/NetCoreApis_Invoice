using ProductApi.Entities;

namespace ProductApi.Contracts
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<CustomerEntity>> GetAllAsync();
        Task<CustomerEntity?> GetByIdAsync(int id);
        Task<CustomerEntity> CreateAsync(CustomerEntity entity);
        Task<bool> UpdateAsync(CustomerEntity entity);
        Task<bool> DeleteAsync(int id);
    }
}
