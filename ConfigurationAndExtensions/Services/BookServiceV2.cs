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

//        public int Count => _repositoryContext.Books.ToList().Count(); // Kitap sayýsýný döndürür.

//        public void CreateBook(Book book)
//        {
//            _repositoryContext.Books.Add(book); // Yeni kitabý veritabanýna ekler.
//            _repositoryContext.SaveChanges(); // Deðiþiklikleri kaydeder.
//        }

//        public void DeleteBook(int id)
//        {
//            var book = _repositoryContext.Books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id);
//            _repositoryContext.Remove(book); // Kitabý kitap listesinden siler.
//            _repositoryContext.SaveChanges();
//        }

//        public Book? GetBookById(int id) => _repositoryContext.Books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id); // ID'ye göre kitabý arar, eðer bulunamazsa hata fýrlatýr.

//        public List<Book> GetBooks() => _repositoryContext.Books.ToList() ?? throw new BookBadRequestException(); // Kitap listesini döndürür, eðer liste boþsa hata fýrlatýr.

//        public Book UpdateBook(int id, Book book)
//        {
//            var existingBook = _repositoryContext.Books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id); // ID'ye göre kitap arar, eðer bulunamazsa hata fýrlatýr.

//            if (book.Title is null || book.Price <= 0) // Kitap baþlýðý boþ veya fiyat negatifse hata fýrlatýr.
//            {
//                throw new BookBadRequestException(book);
//            }

//            existingBook.Title = book.Title;
//            existingBook.Price = book.Price;

//            _repositoryContext.SaveChanges();

//            return existingBook; // Güncellenmiþ kitabý döndürür.
//        }
//    }
//}