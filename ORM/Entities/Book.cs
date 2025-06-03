using ORM.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace ORM.Entities
{
    public class Book
    {
        [Required]
        public int Id { get; set; }

        [Range(10, 100)]
        public decimal Price { get; set; }

        [MinLength(2, ErrorMessage = "Min. lenght must be 2")]
        [MaxLength(250, ErrorMessage = "Max. lenght must be 250")]
        public string? Title { get; set; }
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