namespace IdentityProject.Exceptions.BookExceptions
{
    public sealed class BookNotFoundException : NotFoundException
    {
        public BookNotFoundException(int id) : base($"The book with {id} could not be found!")
        {
        }
    }
}