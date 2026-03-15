using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Contracts;
using ProductApi.Entities;
using System.Diagnostics.Eventing.Reader;

namespace ProductApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ProductEntity>> GetAllAsync()
        { 
            return await _dbContext.Products.ToListAsync();
        }
        public async Task<ProductEntity?> GetByIdAsync(int id)
        {
            return await _dbContext.Products.FindAsync(id);
        }

        public async Task<ProductEntity> CreateAsync(ProductEntity entiry)
        { 
            _dbContext.Products.Add(entiry);    
            await _dbContext.SaveChangesAsync();
            return entiry;
        }
        public async Task<bool> UpdateAsync(ProductEntity entity)
        {
            _dbContext.Products.Update(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbContext.Products.FindAsync(id);
            if (entity == null) 
                return false;

            _dbContext.Products.Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0;             
        }
    }

}
