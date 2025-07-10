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
        private readonly IMapper _mapper; // AutoMapper arayüzü, DTO'larý ve Entity'leri dönüþtürmek için kullanýlýr.

        public BookServiceV3(BookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        #endregion // Constructor

        public int Count => _bookRepository.GetAll().Count; // Kitap sayýsýný döndürür.

        public Book CreateBook(BookDtoForInsertion bookDtoForInsertion) // Yeni kitabý veritabanýna ekler.
        {
            Validate(bookDtoForInsertion); // DTO'yu doðrular. Eðer doðrulama baþarýsýz olursa, ValidationException fýrlatýr.

            var mappedBook = _mapper.Map<Book>(bookDtoForInsertion); // DTO'yu Entity'ye dönüþtürür.

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

            return mappedBook; // Yeni eklenen kitabý döndürür.
        }

        public void DeleteBook(int id)
        {
            id.ValidateIdInRange(); // ID'nin 0'dan büyük ve 1000'den küçük olduðunu kontrol eder. Eðer deðilse, BookBadRequestException fýrlatýr.
            _bookRepository.Delete(id); // Kitabý kitap listesinden siler.
        }

        public BookDTO GetBookById(int id)
        {
            id.ValidateIdInRange(); // ID'nin 0'dan büyük ve 1000'den küçük olduðunu kontrol eder. Eðer deðilse, BookBadRequestException fýrlatýr.

            var book = _bookRepository.Get(id); // ID'ye göre kitabý arar.

            return _mapper.Map<BookDTO>(book);
        }

        public List<BookDTO> GetBooks()
        {
            var books = _bookRepository.GetAll(); // Kitap listesini alýr.

            return _mapper.Map<List<BookDTO>>(books); // Kitap listesini DTO'lara dönüþtürür.
        }

        public Book UpdateBook(int id, BookDtoForUpdate bookDtoForUpdate)
        {
            id.ValidateIdInRange(); // ID'nin 0'dan büyük ve 1000'den küçük olduðunu kontrol eder. Eðer deðilse, BookBadRequestException fýrlatýr.
            Validate(bookDtoForUpdate); // DTO'yu doðrular. Eðer doðrulama baþarýsýz olursa, ValidationException fýrlatýr.

            var mappedBook = _mapper.Map<Book>(bookDtoForUpdate); // DTO'yu Entity'ye dönüþtürür.
            if (mappedBook.Title is null || mappedBook.Price <= 0) // Kitap baþlýðý boþ veya fiyat negatifse hata fýrlatýr.
            {
                throw new BookBadRequestException(mappedBook);
            }

            _bookRepository.Update(id, mappedBook); // Kitabý günceller ve güncellenmiþ kitabý döndürür.

            var updatedBook = _bookRepository.Get(id);
            return updatedBook;
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