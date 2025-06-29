using AutoMapper.Abstracts;
using AutoMapper.Entities;
using AutoMapper.Repositories;
using AutoMapperProject.DTOs;
using System.ComponentModel.DataAnnotations;

namespace AutoMapper.Services
{
    public class BookServiceV3 : IBookService
    {
        #region // Constructor

        private readonly BookRepository _bookRepository;
        private readonly IMapper _mapper; // AutoMapper aray�z�, DTO'lar� ve Entity'leri d�n��t�rmek i�in kullan�l�r.
        public BookServiceV3(BookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        #endregion // Constructor

        public int Count => _bookRepository.GetAll().Count; // Kitap say�s�n� d�nd�r�r.
        public Book CreateBook(BookDtoForInsertion bookDtoForInsertion) // Yeni kitab� veritaban�na ekler.
        {
            var validationResults = new List<ValidationResult>(); // ValidationResult, do�rulama sonu�lar�n� tutar.
            var context = new ValidationContext(bookDtoForInsertion); // ValidationContext, do�rulama ba�lam�n� tutar.
            bool isValid = Validator.TryValidateObject(bookDtoForInsertion, context, validationResults, true); // Validator, do�rulama i�lemini yapar. // TryValidateObject, do�rulama i�lemini yapar ve sonu�lar� validationResults listesine ekler. // true parametresi, t�m �zelliklerin do�rulanmas�n� sa�lar.

            if (!isValid)
            {
                var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage)); // Do�rulama hatalar�n� birle�tirir.
                throw new ValidationException(errors); // Do�rulama hatalar�n� i�eren bir ValidationException f�rlat�r.
            }

            var mappedBook = _mapper.Map<Book>(bookDtoForInsertion); // DTO'yu Entity'ye d�n��t�r�r.

            // Temel alan kontrolleri
            if (string.IsNullOrWhiteSpace(mappedBook.Title) || mappedBook.Price <= 0)
            {
                throw new BookBadRequestException(mappedBook);
            }
            if (mappedBook.Title.Contains("string"))
            {
                throw new BookBadRequestException(mappedBook);
            }

            _bookRepository.Create(mappedBook);

            return mappedBook; // Yeni eklenen kitab� d�nd�r�r.
        }
        public void DeleteBook(int id) => _bookRepository.Delete(id); // Kitab� kitap listesinden siler.
        public Book GetBookById(int id)
        {
            var book = _bookRepository.Get(id); // ID'ye g�re kitab� arar.
            if (book is null) { throw new BookNotFoundException(id); }
            return book;
        }
        public List<Book> GetBooks() => _bookRepository.GetAll(); // Kitap listesini d�nd�r�r.
        public Book UpdateBook(int id, BookDtoForUpdate bookDtoForUpdate)
        {
            if (!(id > 0 && id <= 1000))
            {
                throw new BookBadRequestException(new Book { Id = id, Title = "Invalid ID", Price = 0 }); // E�er ID 0'dan k���k veya 1000'den b�y�kse, hata f�rlat�r.
            }

            var validationResults = new List<ValidationResult>(); // ValidationResult, do�rulama sonu�lar�n� tutar.
            var context = new ValidationContext(bookDtoForUpdate); // ValidationContext, do�rulama ba�lam�n� tutar.
            bool isValid = Validator.TryValidateObject(bookDtoForUpdate, context, validationResults, true); // Validator, do�rulama i�lemini yapar. // TryValidateObject, do�rulama i�lemini yapar ve sonu�lar� validationResults listesine ekler. // true parametresi, t�m �zelliklerin do�rulanmas�n� sa�lar.

            if (!isValid)
            {
                var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage)); // Do�rulama hatalar�n� birle�tirir.
                throw new ValidationException(errors); // Do�rulama hatalar�n� i�eren bir ValidationException f�rlat�r.
            }

            var mappedBook = _mapper.Map<Book>(bookDtoForUpdate); // DTO'yu Entity'ye d�n��t�r�r.

            if (mappedBook.Title is null || mappedBook.Price <= 0) // Kitap ba�l��� bo� veya fiyat negatifse hata f�rlat�r.
            {
                throw new BookBadRequestException(mappedBook);
            }

            _bookRepository.Update(id, mappedBook); // Kitab� g�nceller ve g�ncellenmi� kitab� d�nd�r�r.

            var updatedBook = GetBookById(id);
            return updatedBook;
        }
    }
}