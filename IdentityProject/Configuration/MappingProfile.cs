using AutoMapper;
using IdentityProject.Entities;
using IdentityProject.DTOs.BookDTOs;
using IdentityProject.DTOs.CategoryDTOs;

namespace IdentityProject.Configuration
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