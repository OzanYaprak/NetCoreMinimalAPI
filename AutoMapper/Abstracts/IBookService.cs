using AutoMapper.Entities;
using AutoMapperProject.DTOs;

namespace AutoMapper.Abstracts
{
    public interface IBookService
    {
        public int Count { get; }

        List<Book> GetBooks();

        Book GetBookById(int id);

        Book CreateBook(BookDtoForInsertion bookDtoForInsertion);

        Book UpdateBook(int id, BookDtoForUpdate bookDtoForUpdate);

        void DeleteBook(int id);
    }
}