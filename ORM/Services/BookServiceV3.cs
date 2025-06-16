using ORM.Abstracts;
using ORM.Entities;
using ORM.Repositories;

namespace ORM.Services
{
    public class BookServiceV3 : IBookService
    {
        #region // Constructor

        private readonly BookRepository _bookRepository;

        public BookServiceV3(BookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        #endregion // Constructor

        public int Count => _bookRepository.GetAll().Count; // Kitap sayýsýný döndürür.
        public void CreateBook(Book book) => _bookRepository.Create(book); // Yeni kitabý veritabanýna ekler.
        public void DeleteBook(int id) => _bookRepository.Delete(id); // Kitabý kitap listesinden siler.
        public Book? GetBookById(int id) => _bookRepository.Get(id); // ID'ye göre kitabý arar.
        public List<Book> GetBooks() => _bookRepository.GetAll(); // Kitap listesini döndürür.
        public Book UpdateBook(int id, Book book)
        {
            if (book.Title is null || book.Price <= 0) // Kitap baþlýðý boþ veya fiyat negatifse hata fýrlatýr.
            {
                throw new BookBadRequestException(book);
            }

            _bookRepository.Update(id, book); // Kitabý günceller ve güncellenmiþ kitabý döndürür.

            var updatedBook = _bookRepository.Get(id);
            if (updatedBook is null) { throw new BookNotFoundException(id); }
                
            return updatedBook;
        }
    }
}