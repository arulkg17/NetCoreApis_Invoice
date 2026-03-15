using Microsoft.EntityFrameworkCore;
using ProductApi.Contracts;
using ProductApi.DTOs;
using ProductApi.Entities;
using AutoMapper;

namespace customerApi.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customer;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customer, IMapper mapper)
        {
            _customer = customer;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        { 
            var entities = await _customer.GetAllAsync();
            return _mapper.Map<IEnumerable<CustomerDto>>(entities);
            //return entities.Select(e => new customerDto {
            //    Id = e.Id,
            //    Name = e.Name,
            //    Description = e.Description,
            //    Price = e.Price,
            //    Stock = e.Stock,
            //});
        }
        public async Task<CustomerDto?> GetByIdAsync(int id)
        { 
            var e = await _customer.GetByIdAsync(id);
            if(e == null) return null;
            return _mapper.Map<CustomerDto?>(e);
            //return new customerDto {
            //    Id = e.Id,
            //    Name = e.Name,
            //    Description = e.Description,
            //    Price = e.Price,
            //    Stock = e.Stock,
            //};
        }

        public async Task<CustomerDto> CreateAsync(CustomerDto dto)
        {
            var e = _mapper.Map<CustomerEntity>(dto);

            //var e = new customerEntity
            //{
            //    Name = dto.Name,
            //    Description = dto.Description,
            //    Price = dto.Price,
            //    Stock = dto.Stock
            //};

            var created = await _customer.CreateAsync(e);
            dto = _mapper.Map<CustomerDto>(created);  
            //dto.Id = created.Id;
            return dto;
        }

        public async Task<bool> UpdateAsync(CustomerDto dto)
        {
            var e = _mapper.Map<CustomerEntity>(dto);
            //var e = new customerEntity
            //{
            //    Id = dto.Id,
            //    Name = dto.Name,
            //    Description = dto.Description,
            //    Price = dto.Price,
            //    Stock = dto.Stock
            //};

            return await _customer.UpdateAsync(e);
        }

        public async Task<bool> DeleteAsync(int id)
        { 
            return await _customer.DeleteAsync(id);
        }
    }
}
