using AutoMapper;
using ProductApi.DTOs;
using ProductApi.Entities;

namespace ProductApi.Mapper;

public class ItemmasterProfile : Profile
{
    public ItemmasterProfile() {
        CreateMap<Itemmaster, ItemmasterDto>().ReverseMap();
    }
}
