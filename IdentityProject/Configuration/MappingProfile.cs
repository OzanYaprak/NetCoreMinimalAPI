using AutoMapper;
using IdentityProject.Entities;
using IdentityProject.DTOs.BookDTOs;
using IdentityProject.DTOs.CategoryDTOs;
using IdentityProject.DTOs.IdentityDTOs;

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

            CreateMap<UserDTOForRegistration, User>().ReverseMap();
            CreateMap<AdminDTOForRegistration, User>().ReverseMap();
            CreateMap<UserDTOForAuthentication, User>().ReverseMap();
        }
    }
}