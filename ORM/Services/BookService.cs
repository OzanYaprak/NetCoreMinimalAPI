using ORM.Abstracts;
using ORM.Entities;

namespace ORM.Services
{
    public class BookService : IBookService
    {
        private readonly List<Book> _books;

        public List<Book> GetBooks() => _books; // Kitap listesini d�nd�r�r.

        public int Count => _books.Count; // Kitap say�s�n� d�nd�r�r.

        public Book GetBookById(int id)
        {
            return _books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id);
        }

        public void CreateBook(Book book)
        {
            book.Id = _books.Max(x => x.Id) + 1; // Yeni kitap i�in ID'yi otomatik olarak art�r�r. // Kitap listesindeki en y�ksek ID'ye 1 ekler.
            _books.Add(book); // Yeni kitab� kitap listesine ekler.
        }

        public Book UpdateBook(int id, Book book)
        {
            var existingBook = _books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id); // ID'ye g�re kitap arar, e�er bulunamazsa hata f�rlat�r.
            existingBook.Title = book.Title;
            existingBook.Price = book.Price;

            return existingBook; // G�ncellenmi� kitab� d�nd�r�r.
        }

        public void DeleteBook(int id)
        {
            var book = _books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id);
            _books.Remove(book); // Kitab� kitap listesinden siler.
        }
    }
}