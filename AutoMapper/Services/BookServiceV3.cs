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
            var validationResults = new List<ValidationResult>(); // ValidationResult, doðrulama sonuçlarýný tutar.
            var context = new ValidationContext(bookDtoForInsertion); // ValidationContext, doðrulama baðlamýný tutar.
            bool isValid = Validator.TryValidateObject(bookDtoForInsertion, context, validationResults, true); // Validator, doðrulama iþlemini yapar. // TryValidateObject, doðrulama iþlemini yapar ve sonuçlarý validationResults listesine ekler. // true parametresi, tüm özelliklerin doðrulanmasýný saðlar.

            if (!isValid)
            {
                var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage)); // Doðrulama hatalarýný birleþtirir.
                throw new ValidationException(errors); // Doðrulama hatalarýný içeren bir ValidationException fýrlatýr.
            }

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
        public void DeleteBook(int id) => _bookRepository.Delete(id); // Kitabý kitap listesinden siler.
        public Book GetBookById(int id)
        {
            var book = _bookRepository.Get(id); // ID'ye göre kitabý arar.
            if (book is null) { throw new BookNotFoundException(id); }
            return book;
        }
        public List<Book> GetBooks() => _bookRepository.GetAll(); // Kitap listesini döndürür.
        public Book UpdateBook(int id, BookDtoForUpdate bookDtoForUpdate)
        {
            if (!(id > 0 && id <= 1000))
            {
                throw new BookBadRequestException(new Book { Id = id, Title = "Invalid ID", Price = 0 }); // Eðer ID 0'dan küçük veya 1000'den büyükse, hata fýrlatýr.
            }

            var validationResults = new List<ValidationResult>(); // ValidationResult, doðrulama sonuçlarýný tutar.
            var context = new ValidationContext(bookDtoForUpdate); // ValidationContext, doðrulama baðlamýný tutar.
            bool isValid = Validator.TryValidateObject(bookDtoForUpdate, context, validationResults, true); // Validator, doðrulama iþlemini yapar. // TryValidateObject, doðrulama iþlemini yapar ve sonuçlarý validationResults listesine ekler. // true parametresi, tüm özelliklerin doðrulanmasýný saðlar.

            if (!isValid)
            {
                var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage)); // Doðrulama hatalarýný birleþtirir.
                throw new ValidationException(errors); // Doðrulama hatalarýný içeren bir ValidationException fýrlatýr.
            }

            var mappedBook = _mapper.Map<Book>(bookDtoForUpdate); // DTO'yu Entity'ye dönüþtürür.

            if (mappedBook.Title is null || mappedBook.Price <= 0) // Kitap baþlýðý boþ veya fiyat negatifse hata fýrlatýr.
            {
                throw new BookBadRequestException(mappedBook);
            }

            _bookRepository.Update(id, mappedBook); // Kitabý günceller ve güncellenmiþ kitabý döndürür.

            var updatedBook = GetBookById(id);
            return updatedBook;
        }
    }
}