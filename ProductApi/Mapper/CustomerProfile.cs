using AutoMapper;
using ProductApi.Entities;
using ProductApi.DTOs;

namespace ProductApi.Mapper
{
    public class CustomerProfile:Profile
    {
        public CustomerProfile()
        {
            CreateMap<CustomerEntity, CustomerDto>().ReverseMap();
        }
    }
}
