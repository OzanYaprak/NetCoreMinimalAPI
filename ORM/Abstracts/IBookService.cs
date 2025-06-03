using ORM.Entities;

namespace ORM.Abstracts
{
    public interface IBookService
    {
        public int Count { get; }

        List<Book> GetBooks();

        Book? GetBookById(int id);

        void CreateBook(Book book);

        Book UpdateBook(int id, Book book);

        void DeleteBook(int id);
    }
}