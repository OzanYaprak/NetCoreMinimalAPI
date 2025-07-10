using IdentityProject.Entities;

namespace IdentityProject.Exceptions.CategoryExceptions
{
    // sealed: Bu sınıftan başka bir sınıf türetilemez (miras alınamaz).
    public sealed class CategoryBadRequestException : BadRequestException
    {
        public CategoryBadRequestException(Category category) : base($"Bad Request")
        {
        }

        public CategoryBadRequestException() : base($"Bad Request")
        {
        }
    }
}