using RelationsProject.Repositories.Context;

namespace RelationsProject.Repositories.Base
{
    // • soyut (abstract) bir sınıf tanımlar. Soyut sınıflar, doğrudan örneği (instance) oluşturulamayan, ancak başka sınıflar tarafından miras alınabilen temel sınıflardır.
    //Genellikle ortak özellikler veya metotlar tanımlamak için kullanılırlar.
    public abstract class RepositoryBase<T> where T : class, new() // T, sınıf türünde bir parametre olarak tanımlanır. Bu, RepositoryBase sınıfının yalnızca sınıf türündeki nesnelerle çalışabileceği anlamına gelir.
    {
        #region Constructor

        // protected: Bu alan, sadece bu sınıf ve bundan türeyen (miras alan) sınıflar tarafından erişilebilir.
        protected readonly RepositoryContext _repositoryContext; // RepositoryContext, Entity Framework Core'un DbContext sınıfını temsil eder. Veritabanı bağlantısı ve işlemleri için kullanılır.

        protected RepositoryBase(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        #endregion Constructor

        public virtual T Get(int id) // • virtual: Bu metot, alt sınıflarda (türeyen sınıflarda) istenirse override (geçersiz kılma) edilebilir, yani farklı bir şekilde yeniden yazılabilir.
        {
            return _repositoryContext.Set<T>().Find(id); // Veritabanında belirtilen ID'ye sahip nesneyi bulur. Eğer bulunamazsa null döner.
        }

        public virtual List<T> GetAll() // Tüm nesneleri getirir.
        {
            return _repositoryContext.Set<T>().ToList(); // Veritabanındaki tüm nesneleri liste olarak döndürür.
        }

        public virtual void Create(T entity) // Yeni bir nesne oluşturur.
        {
            _repositoryContext.Set<T>().Add(entity); // Veritabanına yeni nesneyi ekler.
            _repositoryContext.SaveChanges(); // Değişiklikleri kaydeder.
        }

        public virtual void Update(int id, T entity) // Mevcut bir nesneyi günceller.
        {
            var existingEntity = _repositoryContext.Set<T>().Find(id); // Veritabanında belirtilen ID'ye sahip nesneyi bulur.
            if (existingEntity != null)
            {
                _repositoryContext.Entry(existingEntity).CurrentValues.SetValues(entity); // Mevcut nesnenin değerlerini yeni değerlerle günceller.
                _repositoryContext.SaveChanges(); // Değişiklikleri kaydeder.
            }
        }

        public virtual void Delete(int id)
        {
            var entity = _repositoryContext.Set<T>().Find(id); // Veritabanında belirtilen ID'ye sahip nesneyi bulur.
            if (entity != null)
            {
                _repositoryContext.Set<T>().Remove(entity); // Bulunan nesneyi veritabanından siler.
                _repositoryContext.SaveChanges(); // Değişiklikleri kaydeder.
            }
        }
    }
}