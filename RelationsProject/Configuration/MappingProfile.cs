using AutoMapper;
using RelationsProject.Entities;
using RelationsProject.DTOs.BookDTOs;
using RelationsProject.DTOs.CategoryDTOs;

namespace RelationsProject.Configuration
{
    public class MappingProfile : Profile
    {
        // Add your mapping configurations here
        public MappingProfile()
        {
            CreateMap<BookDTO, Book>().ReverseMap();
            CreateMap<BookDtoForInsertion, Book>().ReverseMap();
            CreateMap<BookDtoForUpdate, Book>().ReverseMap();

            CreateMap<CategoryDTO, Category>().ReverseMap();
            CreateMap<CategoryDTOForInsertion, Category>().ReverseMap();
            CreateMap<CategoryDTOForUpdate, Category>().ReverseMap();
        }
    }
}