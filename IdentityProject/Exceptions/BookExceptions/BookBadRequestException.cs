using IdentityProject.Entities;

namespace IdentityProject.Exceptions.BookExceptions
{
    public sealed class BookBadRequestException : BadRequestException
    {
        public BookBadRequestException(Book book) : base($"Bad Request")
        {
        }

        public BookBadRequestException() : base($"Bad Request")
        {
        }
    }
}