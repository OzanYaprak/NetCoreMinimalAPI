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

        public int Count => _bookRepository.GetAll().Count; // Kitap say�s�n� d�nd�r�r.
        public void CreateBook(Book book) => _bookRepository.Create(book); // Yeni kitab� veritaban�na ekler.
        public void DeleteBook(int id) => _bookRepository.Delete(id); // Kitab� kitap listesinden siler.
        public Book? GetBookById(int id) => _bookRepository.Get(id); // ID'ye g�re kitab� arar.
        public List<Book> GetBooks() => _bookRepository.GetAll(); // Kitap listesini d�nd�r�r.
        public Book UpdateBook(int id, Book book)
        {
            if (book.Title is null || book.Price <= 0) // Kitap ba�l��� bo� veya fiyat negatifse hata f�rlat�r.
            {
                throw new BookBadRequestException(book);
            }

            _bookRepository.Update(id, book); // Kitab� g�nceller ve g�ncellenmi� kitab� d�nd�r�r.

            var updatedBook = _bookRepository.Get(id);
            if (updatedBook is null) { throw new BookNotFoundException(id); }
                
            return updatedBook;
        }
    }
}