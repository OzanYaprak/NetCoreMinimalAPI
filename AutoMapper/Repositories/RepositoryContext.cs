using Microsoft.EntityFrameworkCore;
using AutoMapper.Entities;

namespace AutoMapper.Repositories
{
    public class RepositoryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }

        public RepositoryContext(DbContextOptions dbContextOptions) : base(dbContextOptions) // DbContextOptions parametresi, veritabanı bağlantı ayarlarını içerir. // Bu parametre, base(dbContextOptions) aracılığı ile DbContext sınıfının yapıcısına geçirilir.
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "C# Programming", Price = 29.99m },
                new Book { Id = 2, Title = "ASP.NET Core", Price = 39.99m },
                new Book { Id = 3, Title = "Entity Framework Core", Price = 49.99m }
            );
        }
    }
}