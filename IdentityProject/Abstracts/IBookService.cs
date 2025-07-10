using IdentityProject.Entities;
using IdentityProject.DTOs.BookDTOs;

namespace IdentityProject.Abstracts
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