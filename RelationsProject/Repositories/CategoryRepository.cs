using RelationsProject.Entities;
using RelationsProject.Exceptions.CategoryExceptions;
using RelationsProject.Repositories.Base;
using RelationsProject.Repositories.Context;

namespace RelationsProject.Repositories
{
    public class CategoryRepository : RepositoryBase<Category>
    {
        public CategoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}