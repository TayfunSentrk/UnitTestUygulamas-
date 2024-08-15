
using Microsoft.EntityFrameworkCore;
using UnitTestUygulaması.Web.Models;

namespace UnitTestUygulaması.Web.Repository
{
    /// <summary>
    /// Bu sınıf, genel bir varlık tipi için CRUD işlemlerini gerçekleştiren bir repository sınıfıdır.
    /// </summary>
    /// <typeparam name="T">Varlık tipi. Sadece sınıf türü olabilir.</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly UnitTestDbContext unitTestDbContext;
        private readonly DbSet<T> dbSet;

        /// <summary>
        /// Repository sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="unitTestDbContext">Veritabanı bağlamı (DbContext).</param>
        public Repository(UnitTestDbContext unitTestDbContext)
        {
            this.unitTestDbContext = unitTestDbContext;
            this.dbSet = unitTestDbContext.Set<T>();
        }

        /// <summary>
        /// Yeni bir varlık oluşturur ve veritabanına ekler.
        /// </summary>
        /// <param name="entity">Oluşturulacak varlık.</param>
        public async Task Create(T entity)
        {
            await dbSet.AddAsync(entity);
            await unitTestDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Mevcut bir varlığı veritabanından siler.
        /// </summary>
        /// <param name="entity">Silinecek varlık.</param>
        public void Delete(T entity)
        {
            dbSet.Remove(entity);
            unitTestDbContext.SaveChanges();
        }

        /// <summary>
        /// Tüm varlıkları asenkron olarak getirir.
        /// </summary>
        /// <returns>Varlıkların bir koleksiyonunu içeren bir görev.</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        /// <summary>
        /// Verilen kimliğe sahip bir varlığı asenkron olarak getirir.
        /// </summary>
        /// <param name="id">Varlığın kimliği.</param>
        /// <returns>Belirtilen kimliğe sahip varlığı içeren bir görev.</returns>
        public async Task<T> GetAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        /// <summary>
        /// Mevcut bir varlığı günceller.
        /// </summary>
        /// <param name="entity">Güncellenecek varlık.</param>
        public void Update(T entity)
        {
            dbSet.Update(entity);
            unitTestDbContext.SaveChanges();
        }
    }

}
