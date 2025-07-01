namespace RelationsProject.Exceptions.CategoryExceptions
{
    // sealed: Bu sınıftan başka bir sınıf türetilemez (miras alınamaz).
    public sealed class CategoryNotFoundException : NotFoundException
    {
        public CategoryNotFoundException(int id) : base($"The category with {id} could not be found!")
        {
        }
    }
}