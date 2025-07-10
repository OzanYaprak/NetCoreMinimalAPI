using Microsoft.EntityFrameworkCore;
using IdentityProject.Entities;
using System.Security.AccessControl;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IdentityProject.Repositories.Context
{
    //public class RepositoryContext : DbContext
    public class RepositoryContext : IdentityDbContext<User> // IdentityDbContext, ASP.NET Core Identity ile entegrasyon sağlar. User sınıfı, IdentityUser sınıfından türetilmiştir ve kullanıcı bilgilerini içerir.
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }

        public RepositoryContext(DbContextOptions dbContextOptions) : base(dbContextOptions) // DbContextOptions parametresi, veritabanı bağlantı ayarlarını içerir. // Bu parametre, base(dbContextOptions) aracılığı ile DbContext sınıfının yapıcısına geçirilir.
        {
        }

        // Model oluşturma işlemi sırasında çağrılır. Bu metot, veritabanı tablolarının yapısını ve ilişkilerini tanımlamak için kullanılır.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    CategoryId = 1,
                    CategoryName = "Programming"
                },
                new Category
                {
                    CategoryId = 2,
                    CategoryName = "Web Development"
                },
                new Category
                {
                    CategoryId = 3,
                    CategoryName = "Database"
                }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    URL = "/images/1.jpg",
                    Title = "C# Programming",
                    Price = 29.99m,
                    CategoryId = 1 // C# Programming kitabı, "Programming" kategorisine aittir.
                },
                new Book
                {
                    Id = 2,
                    URL = "/images/2.jpg",
                    Title = "ASP.NET Core",
                    Price = 39.99m,
                    CategoryId = 2 // ASP.NET Core kitabı, "Web Development" kategorisine aittir.
                },
                new Book
                {
                    Id = 3,
                    URL = "/images/3.jpg",
                    Title = "Entity Framework Core",
                    Price = 49.99m,
                    CategoryId = 3 // Entity Framework Core kitabı, "Database" kategorisine aittir.
                }
            );
        }
    }
}