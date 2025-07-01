using Microsoft.EntityFrameworkCore;
using RelationsProject.Entities;
using RelationsProject.Exceptions.BookExceptions;
using RelationsProject.Repositories.Base;
using RelationsProject.Repositories.Context;

namespace RelationsProject.Repositories
{
    public class BookRepository : RepositoryBase<Book>
    {
        public BookRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        #region Methods

        public override Book Get(int id) // RepositoryBase den farklı olarak bu metot'da Eager Loading kullanarak Category bilgisini de getirir. // Bu yüzden override edilmiştir.
        {
            return _repositoryContext.Books
                .Include(x => x.Category) // eager loading ile Category bilgisini de getirir.
                .FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id); // ID'ye göre kitabı arar, eğer bulunamazsa hata fırlatır.
        }

        public override List<Book> GetAll() // RepositoryBase den farklı olarak bu metot'da Eager Loading kullanarak Category bilgisini de getirir. // Bu yüzden override edilmiştir.
        {
            return _repositoryContext.Books
                .Include(x => x.Category) // Eager loading ile Category bilgisini de getirir.
                .ToList() ?? throw new BookBadRequestException(); // Kitap listesini döndürür, eğer liste boşsa hata fırlatır.
        }

        #endregion Methods
    }
}