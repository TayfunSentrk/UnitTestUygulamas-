using Microsoft.AspNetCore.Mvc;
using UnitTestUygulaması.Web.Models;
using UnitTestUygulaması.Web.Repository;

namespace UnitTestUygulaması.Web.Controllers
{
    /// <summary>
    /// Ürünler için temel CRUD işlemlerini gerçekleştiren kontrolör sınıfı.
    /// </summary>
    public class ProductsController : Controller
    {
        private readonly IRepository<Product> _repository;

        /// <summary>
        /// Kontrolörün yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="repository">Ürünler için kullanılan repository.</param>
        public ProductsController(IRepository<Product> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Ürünlerin listesini getirir.
        /// </summary>
        /// <returns>Ürünlerin listelendiği Index görünümü.</returns>
        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetAllAsync());
        }

        /// <summary>
        /// Belirtilen kimliğe sahip ürünün detaylarını getirir.
        /// </summary>
        /// <param name="id">Ürün kimliği.</param>
        /// <returns>Ürün detaylarının gösterildiği Details görünümü.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var product = await _repository.GetAsync((int)id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        /// <summary>
        /// Yeni bir ürün oluşturma formunu getirir.
        /// </summary>
        /// <returns>Ürün oluşturma formunu içeren Create görünümü.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Yeni bir ürün oluşturur.
        /// </summary>
        /// <param name="product">Oluşturulacak ürün.</param>
        /// <returns>Başarılıysa Index sayfasına yönlendirir, aksi takdirde Create görünümünü tekrar döndürür.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Stock,Color")] Product product)
        {
            if (ModelState.IsValid)
            {
                await _repository.Create(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        /// <summary>
        /// Düzenlenecek ürünün formunu getirir.
        /// </summary>
        /// <param name="id">Düzenlenecek ürünün kimliği.</param>
        /// <returns>Ürün düzenleme formunu içeren Edit görünümü.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var product = await _repository.GetAsync((int)id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        /// <summary>
        /// Mevcut bir ürünü günceller.
        /// </summary>
        /// <param name="id">Güncellenecek ürünün kimliği.</param>
        /// <param name="product">Güncellenmiş ürün bilgileri.</param>
        /// <returns>Başarılıysa Index sayfasına yönlendirir, aksi takdirde Edit görünümünü tekrar döndürür.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Price,Stock,Color")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _repository.Update(product);

                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        /// <summary>
        /// Silinecek ürünün detaylarını getirir.
        /// </summary>
        /// <param name="id">Silinecek ürünün kimliği.</param>
        /// <returns>Ürün silme onay formunu içeren Delete görünümü.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _repository.GetAsync((int)id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        /// <summary>
        /// Ürünü siler.
        /// </summary>
        /// <param name="id">Silinecek ürünün kimliği.</param>
        /// <returns>Başarılıysa Index sayfasına yönlendirir.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _repository.GetAsync(id);

            _repository.Delete(product);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Belirtilen kimliğe sahip bir ürün olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="id">Ürün kimliği.</param>
        /// <returns>Ürün varsa true, yoksa false döner.</returns>
        public bool ProductExists(int id)
        {
            var product = _repository.GetAsync(id).Result;

            if (product == null)
                return false;
            else
                return true;
        }


    }

}
