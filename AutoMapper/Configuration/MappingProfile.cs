using AutoMapper;
using AutoMapper.Entities;
using AutoMapperProject.DTOs;

namespace AutoMapperProject.Configuration
{
    public class MappingProfile : Profile
    {
        // Add your mapping configurations here
        public MappingProfile()
        {
            CreateMap<BookDtoForInsertion, Book>().ReverseMap();
            CreateMap<BookDtoForUpdate, Book>().ReverseMap();
        }
    }
}
