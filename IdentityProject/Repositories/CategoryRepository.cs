using IdentityProject.Entities;
using IdentityProject.Exceptions.CategoryExceptions;
using IdentityProject.Repositories.Base;
using IdentityProject.Repositories.Context;

namespace IdentityProject.Repositories
{
    public class CategoryRepository : RepositoryBase<Category>
    {
        public CategoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}