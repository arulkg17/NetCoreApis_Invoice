using AutoMapper;
using ProductApi.Entities;
using ProductApi.DTOs;

namespace ProductApi.Mapper
{
    public class ProductProfile:Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductEntity, ProductDto>().ReverseMap();
        }
    }
}
