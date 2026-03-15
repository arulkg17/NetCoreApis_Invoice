using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Contracts;
using ProductApi.Entities;
using System.Diagnostics.Eventing.Reader;

namespace ProductApi.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _dbContext;

        public CustomerRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<CustomerEntity>> GetAllAsync()
        { 
            return await _dbContext.Customers.ToListAsync();
        }
        public async Task<CustomerEntity?> GetByIdAsync(int id)
        {
            return await _dbContext.Customers.FindAsync(id);
        }

        public async Task<CustomerEntity> CreateAsync(CustomerEntity entiry)
        { 
            _dbContext.Customers.Add(entiry);    
            await _dbContext.SaveChangesAsync();
            return entiry;
        }
        public async Task<bool> UpdateAsync(CustomerEntity entity)
        {
            _dbContext.Customers.Update(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbContext.Customers.FindAsync(id);
            if (entity == null) 
                return false;

            _dbContext.Customers.Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0;             
        }
    }

}
