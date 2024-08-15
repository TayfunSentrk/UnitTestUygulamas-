namespace UnitTestUygulaması.Web.Repository
{
    /// <summary>
    /// Bu arayüz, genel bir varlık için temel CRUD (Oluşturma, Okuma, Güncelleme, Silme) işlemlerini tanımlar.
    /// </summary>
    /// <typeparam name="T">Varlık tipi.</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Asenkron olarak tüm T tipi varlıkları döndürür.
        /// </summary>
        /// <returns>Asenkron işlemi temsil eden bir görev. Görev sonucu, varlıkların bir koleksiyonunu içerir.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Asenkron olarak, kimliği belirtilen T tipi bir varlığı döndürür.
        /// </summary>
        /// <param name="id">Varlığın kimliği.</param>
        /// <returns>Asenkron işlemi temsil eden bir görev.</returns>
        Task<T> GetAsync(int id);

        /// <summary>
        /// Yeni bir T tipi varlık oluşturur.
        /// </summary>
        /// <param name="entity">Oluşturulacak varlık.</param>
        Task Create(T entity);

        /// <summary>
        /// Mevcut bir T tipi varlığı günceller.
        /// </summary>
        /// <param name="entity">Güncellenecek varlık.</param>
        void Update(T entity);

        /// <summary>
        /// Mevcut bir T tipi varlığı siler.
        /// </summary>
        /// <param name="entity">Silinecek varlık.</param>
        void Delete(T entity);
    }
}
