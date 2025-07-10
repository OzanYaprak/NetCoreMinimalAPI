using IdentityProject.Entities;
using IdentityProject.DTOs.CategoryDTOs;

namespace IdentityProject.Abstracts
{
    public interface ICategoryService
    {
        public int Count { get; }

        List<CategoryDTO> GetCategories();

        CategoryDTO GetCategoryById(int id);

        Category CreateCategory(CategoryDTOForInsertion categoryDTOForInsertion);

        Category UpdateCategory(int id, CategoryDTOForUpdate categoryDTOForUpdate);

        void DeleteCategory(int id);
    }
}