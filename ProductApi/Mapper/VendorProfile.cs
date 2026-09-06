using AutoMapper;
using ProductApi.Entities;
using ProductApi.DTOs;

namespace ProductApi.Mapper
{
    public class VendorProfile : Profile
    {
        public VendorProfile()
        {
            CreateMap<VendorEntity, VendorDto>().ReverseMap();
        }
    }
}