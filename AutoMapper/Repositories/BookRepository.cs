using AutoMapper.Entities;

namespace AutoMapper.Repositories
{
    public class BookRepository
    {
        #region Constructor

        private readonly RepositoryContext _repositoryContext;

        public BookRepository(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        #endregion Constructor

        #region Methods

        public Book? Get(int id)
        {
            return _repositoryContext.Books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id); // ID'ye göre kitabı arar, eğer bulunamazsa hata fırlatır.
        }

        public List<Book> GetAll()
        {
            return _repositoryContext.Books.ToList() ?? throw new BookBadRequestException(); // Kitap listesini döndürür, eğer liste boşsa hata fırlatır.
        }

        public void Create(Book book)
        {
            _repositoryContext.Books.Add(book); // Yeni kitabı veritabanına ekler.
            _repositoryContext.SaveChanges(); // Değişiklikleri kaydeder.
        }

        public void Update(int id, Book book)
        {
            var existingBook =_repositoryContext.Books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id); // ID'ye göre kitap arar, eğer bulunamazsa hata fırlatır.
            
            _repositoryContext.Entry(existingBook).CurrentValues.SetValues(book); // Mevcut kitabın değerlerini günceller. 
            _repositoryContext.SaveChanges(); // Değişiklikleri kaydeder.
        }

        public void Delete(int id)
        {
            _repositoryContext.Books.Remove(_repositoryContext.Books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id)); // Kitabı kitap listesinden siler.
            _repositoryContext.SaveChanges();
        }

        #endregion Methods
    }
}
