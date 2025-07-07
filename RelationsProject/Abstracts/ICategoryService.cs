using RelationsProject.Entities;
using RelationsProject.DTOs.CategoryDTOs;

namespace RelationsProject.Abstracts
{
    public interface ICategoryService
    {
        List<CategoryDTO> GetCategories();

        CategoryDTO GetCategoryById(int id);

        Category CreateCategory(CategoryDTOForInsertion categoryDTOForInsertion);

        Category UpdateCategory(int id, CategoryDTOForUpdate categoryDTOForUpdate);

        void DeleteCategory(int id);
    }
}