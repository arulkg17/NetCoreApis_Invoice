using AutoMapper;
using ProductApi.Entities;
using ProductApi.DTOs;
namespace ProductApi.Mapper;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<CategoryEntity, CategoryDto>().ReverseMap();
    }
}