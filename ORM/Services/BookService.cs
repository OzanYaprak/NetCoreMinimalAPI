using ORM.Abstracts;
using ORM.Entities;

namespace ORM.Services
{
    public class BookService : IBookService
    {
        private readonly List<Book> _books;

        public List<Book> GetBooks() => _books; // Kitap listesini döndürür.

        public int Count => _books.Count; // Kitap sayýsýný döndürür.

        public Book GetBookById(int id)
        {
            return _books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id);
        }

        public void CreateBook(Book book)
        {
            book.Id = _books.Max(x => x.Id) + 1; // Yeni kitap için ID'yi otomatik olarak artýrýr. // Kitap listesindeki en yüksek ID'ye 1 ekler.
            _books.Add(book); // Yeni kitabý kitap listesine ekler.
        }

        public Book UpdateBook(int id, Book book)
        {
            var existingBook = _books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id); // ID'ye göre kitap arar, eðer bulunamazsa hata fýrlatýr.
            existingBook.Title = book.Title;
            existingBook.Price = book.Price;

            return existingBook; // Güncellenmiþ kitabý döndürür.
        }

        public void DeleteBook(int id)
        {
            var book = _books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id);
            _books.Remove(book); // Kitabý kitap listesinden siler.
        }
    }
}