using RelationsProject.Entities;
using RelationsProject.DTOs.BookDTOs;

namespace RelationsProject.Abstracts
{
    public interface IBookService
    {
        public int Count { get; }

        List<BookDTO> GetBooks();

        BookDTO GetBookById(int id);

        Book CreateBook(BookDtoForInsertion bookDtoForInsertion);

        Book UpdateBook(int id, BookDtoForUpdate bookDtoForUpdate);

        void DeleteBook(int id);
    }
}