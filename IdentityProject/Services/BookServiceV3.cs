using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using IdentityProject.Abstracts;
using IdentityProject.ConfigurationExtensions;
using IdentityProject.DTOs.BookDTOs;
using IdentityProject.Entities;
using IdentityProject.Exceptions.BookExceptions;
using IdentityProject.Repositories;
using System.ComponentModel.DataAnnotations;

namespace IdentityProject.Services
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
            Validate(bookDtoForInsertion); // DTO'yu do�rular. E�er do�rulama ba�ar�s�z olursa, ValidationException f�rlat�r.

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

        public void DeleteBook(int id)
        {
            id.ValidateIdInRange(); // ID'nin 0'dan b�y�k ve 1000'den k���k oldu�unu kontrol eder. E�er de�ilse, BookBadRequestException f�rlat�r.
            _bookRepository.Delete(id); // Kitab� kitap listesinden siler.
        }

        public BookDTO GetBookById(int id)
        {
            id.ValidateIdInRange(); // ID'nin 0'dan b�y�k ve 1000'den k���k oldu�unu kontrol eder. E�er de�ilse, BookBadRequestException f�rlat�r.

            var book = _bookRepository.Get(id); // ID'ye g�re kitab� arar.

            return _mapper.Map<BookDTO>(book);
        }

        public List<BookDTO> GetBooks()
        {
            var books = _bookRepository.GetAll(); // Kitap listesini al�r.

            return _mapper.Map<List<BookDTO>>(books); // Kitap listesini DTO'lara d�n��t�r�r.
        }

        public Book UpdateBook(int id, BookDtoForUpdate bookDtoForUpdate)
        {
            id.ValidateIdInRange(); // ID'nin 0'dan b�y�k ve 1000'den k���k oldu�unu kontrol eder. E�er de�ilse, BookBadRequestException f�rlat�r.
            Validate(bookDtoForUpdate); // DTO'yu do�rular. E�er do�rulama ba�ar�s�z olursa, ValidationException f�rlat�r.

            var mappedBook = _mapper.Map<Book>(bookDtoForUpdate); // DTO'yu Entity'ye d�n��t�r�r.
            if (mappedBook.Title is null || mappedBook.Price <= 0) // Kitap ba�l��� bo� veya fiyat negatifse hata f�rlat�r.
            {
                throw new BookBadRequestException(mappedBook);
            }

            _bookRepository.Update(id, mappedBook); // Kitab� g�nceller ve g�ncellenmi� kitab� d�nd�r�r.

            var updatedBook = _bookRepository.Get(id);
            return updatedBook;
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