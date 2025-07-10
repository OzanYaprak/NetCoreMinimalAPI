using AutoMapper;
using IdentityProject.Abstracts;
using IdentityProject.ConfigurationExtensions;
using IdentityProject.DTOs.CategoryDTOs;
using IdentityProject.Entities;
using IdentityProject.Exceptions.CategoryExceptions;
using IdentityProject.Repositories;
using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Services
{
    public class CategoryService : ICategoryService
    {
        #region // Constructor

        private readonly CategoryRepository _categoryRepository;
        private readonly IMapper _mapper; // AutoMapper arayüzü, DTO'larý ve Entity'leri dönüþtürmek için kullanýlýr.

        public CategoryService(CategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        #endregion // Constructor

        public int Count => _categoryRepository.GetAll().Count; // Kitap sayýsýný döndürür.

        public Category CreateCategory(CategoryDTOForInsertion categoryDTOForInsertion)
        {
            Validate(categoryDTOForInsertion);
            var mappedCategory = _mapper.Map<Category>(categoryDTOForInsertion); // DTO'yu Entity'ye dönüþtürür.

            if (string.IsNullOrEmpty(categoryDTOForInsertion.CategoryName)) { throw new CategoryBadRequestException(mappedCategory); }

            _categoryRepository.Create(mappedCategory); // Yeni kategoriyi veritabanýna ekler.
            return mappedCategory; // Yeni eklenen kategoriyi döndürür.
        }

        public void DeleteCategory(int id)
        {
            _categoryRepository.Delete(id); // Kitabý kitap listesinden siler.
        }

        public List<CategoryDTO> GetCategories()
        {
            var categories = _categoryRepository.GetAll(); // Kitap listesini alýr.

            return _mapper.Map<List<CategoryDTO>>(categories); // Kitap listesini DTO'lara dönüþtürür.
        }

        public CategoryDTO GetCategoryById(int id)
        {
            var category = _categoryRepository.Get(id);
            return _mapper.Map<CategoryDTO>(category);
        }

        public Category UpdateCategory(int id, CategoryDTOForUpdate categoryDTOForUpdate)
        {
            Validate(categoryDTOForUpdate);

            var mappedCategory = _mapper.Map<Category>(categoryDTOForUpdate);
            if (string.IsNullOrEmpty(mappedCategory.CategoryName))
            {
                throw new CategoryBadRequestException(mappedCategory);
            }

            _categoryRepository.Update(id, mappedCategory);

            var updatedCategory = _categoryRepository.Get(id);
            return updatedCategory;
        }

        private void Validate<T>(T item)
        {
            var validationResults = new List<ValidationResult>(); // ValidationResult, doðrulama sonuçlarýný tutar.
            var context = new ValidationContext(item); // ValidationContext, doðrulama baðlamýný tutar.
            var isValid = Validator.TryValidateObject(item, context, validationResults, true); // Validator, doðrulama iþlemini yapar. // TryValidateObject, doðrulama iþlemini yapar ve sonuçlarý validationResults listesine ekler. // true parametresi, tüm özelliklerin doðrulanmasýný saðlar.

            if (!isValid)
            {
                var errors = string.Join(", ", validationResults.Select(select => select.ErrorMessage)); // Doðrulama hatalarýný birleþtirir.
                throw new ValidationException(errors); // Doðrulama hatalarýný içeren bir ValidationException fýrlatýr.
            }
        }
    }
}