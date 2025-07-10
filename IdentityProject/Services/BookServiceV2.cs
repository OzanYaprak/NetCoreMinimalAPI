//using AutoMapper.Abstracts;
//using AutoMapper.Entities;
//using AutoMapper.Repositories;

//namespace AutoMapper.Services
//{

//    public class BookServiceV2 : IBookService
//    {
//        #region // Constructor

//        private readonly RepositoryContext _repositoryContext;

//        public BookServiceV2(RepositoryContext repositoryContext)
//        {
//            _repositoryContext = repositoryContext;
//        }

//        #endregion // Constructor

//        public int Count => _repositoryContext.Books.ToList().Count(); // Kitap say�s�n� d�nd�r�r.

//        public void CreateBook(Book book)
//        {
//            _repositoryContext.Books.Add(book); // Yeni kitab� veritaban�na ekler.
//            _repositoryContext.SaveChanges(); // De�i�iklikleri kaydeder.
//        }

//        public void DeleteBook(int id)
//        {
//            var book = _repositoryContext.Books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id);
//            _repositoryContext.Remove(book); // Kitab� kitap listesinden siler.
//            _repositoryContext.SaveChanges();
//        }

//        public Book? GetBookById(int id) => _repositoryContext.Books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id); // ID'ye g�re kitab� arar, e�er bulunamazsa hata f�rlat�r.

//        public List<Book> GetBooks() => _repositoryContext.Books.ToList() ?? throw new BookBadRequestException(); // Kitap listesini d�nd�r�r, e�er liste bo�sa hata f�rlat�r.

//        public Book UpdateBook(int id, Book book)
//        {
//            var existingBook = _repositoryContext.Books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id); // ID'ye g�re kitap arar, e�er bulunamazsa hata f�rlat�r.

//            if (book.Title is null || book.Price <= 0) // Kitap ba�l��� bo� veya fiyat negatifse hata f�rlat�r.
//            {
//                throw new BookBadRequestException(book);
//            }

//            existingBook.Title = book.Title;
//            existingBook.Price = book.Price;

//            _repositoryContext.SaveChanges();

//            return existingBook; // G�ncellenmi� kitab� d�nd�r�r.
//        }
//    }
//}