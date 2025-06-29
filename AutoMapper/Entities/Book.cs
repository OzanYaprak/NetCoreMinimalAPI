using AutoMapper.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace AutoMapper.Entities
{
    public class Book
    {
        [Required]
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Title { get; set; }
    }

    public sealed class BookBadRequestException : BadRequestException
    {
        public BookBadRequestException(Book book) : base($"Bad Request")
        {
        }

        public BookBadRequestException() : base($"Bad Request")
        {
        }
    }

    public sealed class BookNotFoundException : NotFoundException
    {
        public BookNotFoundException(int id) : base($"The book with {id} could not be found!")
        {
        }
    }
}