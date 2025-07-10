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
        private readonly IMapper _mapper; // AutoMapper aray�z�, DTO'lar� ve Entity'leri d�n��t�rmek i�in kullan�l�r.

        public CategoryService(CategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        #endregion // Constructor

        public int Count => _categoryRepository.GetAll().Count; // Kitap say�s�n� d�nd�r�r.

        public Category CreateCategory(CategoryDTOForInsertion categoryDTOForInsertion)
        {
            Validate(categoryDTOForInsertion);
            var mappedCategory = _mapper.Map<Category>(categoryDTOForInsertion); // DTO'yu Entity'ye d�n��t�r�r.

            if (string.IsNullOrEmpty(categoryDTOForInsertion.CategoryName)) { throw new CategoryBadRequestException(mappedCategory); }

            _categoryRepository.Create(mappedCategory); // Yeni kategoriyi veritaban�na ekler.
            return mappedCategory; // Yeni eklenen kategoriyi d�nd�r�r.
        }

        public void DeleteCategory(int id)
        {
            _categoryRepository.Delete(id); // Kitab� kitap listesinden siler.
        }

        public List<CategoryDTO> GetCategories()
        {
            var categories = _categoryRepository.GetAll(); // Kitap listesini al�r.

            return _mapper.Map<List<CategoryDTO>>(categories); // Kitap listesini DTO'lara d�n��t�r�r.
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
            var validationResults = new List<ValidationResult>(); // ValidationResult, do�rulama sonu�lar�n� tutar.
            var context = new ValidationContext(item); // ValidationContext, do�rulama ba�lam�n� tutar.
            var isValid = Validator.TryValidateObject(item, context, validationResults, true); // Validator, do�rulama i�lemini yapar. // TryValidateObject, do�rulama i�lemini yapar ve sonu�lar� validationResults listesine ekler. // true parametresi, t�m �zelliklerin do�rulanmas�n� sa�lar.

            if (!isValid)
            {
                var errors = string.Join(", ", validationResults.Select(select => select.ErrorMessage)); // Do�rulama hatalar�n� birle�tirir.
                throw new ValidationException(errors); // Do�rulama hatalar�n� i�eren bir ValidationException f�rlat�r.
            }
        }
    }
}