using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestUygulaması.Web.Controllers;
using UnitTestUygulaması.Web.Models;
using UnitTestUygulaması.Web.Repository;

namespace UnitTestUygulaması.Test
{
    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;//Product Contoroller Bağımlılık olarak IRepository aldığı için Mock Tarafında Onu ilave ettim
        private readonly ProductsController controller;//Bu sınıf içinde methodlar test edilecek

        private List<Product> _products;

        public ProductControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            this.controller = new ProductsController(_mockRepo.Object);//Product contoller constructor tarafında IRepository<Product> beklediği için bu yapıyı kullandım
            _products = new List<Product>() { 
                new Product() { Id = 1, Name = "Kalem", Price = 100, Stock = 50, Color = "Kırmızı" },
                new Product() { Id = 2, Name = "Defter", Price = 200, Stock = 500, Color = "Mavi" }
            };
        }


        //Paremetre almadığı için fact kullandım
        //View Result dönüyor mu diye kontrol edildi

        [Fact]
        public async void Index_ActionExecutes_ReturnView()
        {
            var result=await controller.Index();
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public async void Index_ActionExecutes_ReturnProductList()
        {
            _mockRepo.Setup(m => m.GetAllAsync()).ReturnsAsync(_products);//GetAllAsync fonskiyonu çağırıldığında _product değişkenindeki değerler dönücek

            var result=await controller.Index();    

            var viewResult=Assert.IsType<ViewResult>(result);//viewResult dönüyor mu o kontrol edildi
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model); //product list şeklinde dönmesi sağlandı.
            Assert.Equal<int>(2, productList.Count());// Product Listesi 2 adet mi o kontrol edildi
        }

        [Fact]  
        public async void Details_IdIsNull_ReturnRedirectToIndexAction()
        {
            //IAction Result Dönüyor
            var result = await controller.Details(null);

            //Null verdiğimizde dönüş tipi RedirectActionResult mu o okontrol ediliyor
            var redirect=Assert.IsType<RedirectToActionResult>(result);

            //Action İsmi Index mi o kontrol ediliyor
            Assert.Equal("Index", redirect.ActionName);
        }


        [Fact]

        public async void Details_IdInValid_ReturnNotFound()
        {
            Product product = null;
            //eğer verilen id veritabanında yoksa örnek olarak verilen product gibi null gelmesi sağlandı
            _mockRepo.Setup(m => m.GetAsync(0)).ReturnsAsync(product);

            //IactionResult Tipinde döner
            var result = await controller.Details(0);
            //Tipi NotFoundResult mi diye kontrol edildi
            var redirect =Assert.IsType<NotFoundResult>(result);
            //Status Code 404 mi diye kontrol edildi
            Assert.Equal<int>(404, redirect.StatusCode);
          
        }


        [Theory]
        [InlineData(1)]
        public async void Details_ValidId_ReturnProduct(int id)
        {
            var product = _products.First(p => p.Id == id);//verilen id ye göre elimizde var olan _product listesinde ilk id 'le aynı olan ürün getiriliyor
            _mockRepo.Setup(m=>m.GetAsync(id)).ReturnsAsync(product); //Eğer getAsync methodu çalıştırılırsa geriye product dönüyor  

            var result = await controller.Details(id);  //IactionResult tipinde döner

            var viewResult=Assert.IsType<ViewResult>(result);//ViewResult dönüyor mu kontrol ediliyor
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model); //Model Product tipinde mi kontrol ediliyor
            Assert.Equal(product.Id, resultProduct.Id);//Beklenen ıd ile gerçekleşen Id aynı mı kontrol ediliyor
            Assert.Equal(product.Name, resultProduct.Name);//Beklenen isim ile gerçekleşen isim aynı mı kontrol ediliyor
        }

        [Fact]

        public  void Create_ActionExecutes_ReturnView()
        {
            var result = controller.Create(); //Create get methodu çalıştırılıyor

            Assert.IsType<ViewResult>(result); //Dönüş tipi ViewResult olacak şekilde geliyor mu diye bakılıyor
        }
        [Fact]
        public async void Create_InvalidModelState_ReturnView()
        {
            controller.ModelState.AddModelError("Name", "Name alanı gereklidir");//Burda invalid modelstate olması için hata fırlattım

            var result=await controller.Create(_products.First()); //Daha önce oluşturulmuş products listesinde ilk elemanı ekliyorum

            var viewResult = Assert.IsType<ViewResult>(result);//Result tipi ViewResult şeklinde mi oluyor o kontrol edildi
            Assert.IsType<Product>(viewResult.Model);//Modelden gelen veri Product tipinde;

        }

        [Fact]
       public async void Create_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await controller.Create(_products.First());

            var redirect=Assert.IsType<RedirectToActionResult>(result);//RedirectToActionResult Tipinde mi kontrol ettim
                
            Assert.Equal("Index",redirect.ActionName);//Gittiği yer Index isminde mi kontrol ettim
        }

        [Fact]

        public async void Create_ValidModelState_CreateMethodExecute()
        {
            //Modelsatate doğrıysa create methodu çalışıyor mu test etmek için

            Product newProduct = null;
            _mockRepo.Setup(repo=>repo.Create(It.IsAny<Product>())).Callback<Product>(x=>newProduct=x);//Burda herhangi bir product gönderilirse newProduct nesnesine eklenen product setlencek
            var result = await controller.Create(_products.First());//Create methodu çağırılıyor

            _mockRepo.Verify(repo=>repo.Create(It.IsAny<Product>()), Times.Once);//Create methodu bir kez çalışıyor mı kontrol ettim
            Assert.Equal(_products.First().Id, newProduct.Id);//product listesinden ilk elamanın Id'si ile newProduct.Id aynı diye kontrol ettim
        }


        [Fact]

        public async void Create_InvalidModelState_NeverCreateExecute()
        {
            controller.ModelState.AddModelError("Name", "");//Burda AddModelError ile modelstate'de hata fırlatıyorum

            var result = await controller.Create(_products.First());

            _mockRepo.Verify(repo=>repo.Create(It.IsAny<Product>()), Times.Never);//Çalışmadığını kontrol ediyorum //Burda repository'den gelen method çalışmıyor mu diye kontrol ediyorum

        }


        [Fact]
        public async void Edit_IdIsNull_ReturnRedirectToIndexAction()
        {

            var result = await controller.Edit(null); //id null geldiği durumda edit methodu test ediliyor

            var redirect = Assert.IsType<RedirectToActionResult>(result);//Bu Method çalıştığında RedirectActionResult dönüyor mu kontrol ediliyor

            Assert.Equal("Index", redirect.ActionName); //Gittiği yer Index sayfası mı o kontrol ediliyor

        }

        [Theory]
        [InlineData(3)]
        public async void Edit_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null; //Burda id eğer bulunmayan bir product Id ise null veri getirmek için
            _mockRepo.Setup(x => x.GetAsync(productId)).ReturnsAsync(product);//Burda GetAsync methodu çağırıldığında null olan product olarak dönsün

            var result=await controller.Edit(productId); //edit methodu çalıştırılıyor

            var redirect=Assert.IsType<NotFoundResult>(result); //Dönüş tipi NotFoundResult tipinde mi kontrol ediliyor

            Assert.Equal<int>(404, redirect.StatusCode); //Status Code 404 şeklinde mi kontrol ediliyor
        }

        [Theory]

        [InlineData(1)]

        public async void Edit_IdValid_ReturnProduct(int productId)
        {

            var product = _products.First(x=>x.Id==productId);//product tipinde bir değişken tanımlanarak _product listesinden parametre olarak verdiğimiz id ile eşleşen product bulduk

            _mockRepo.Setup(x => x.GetAsync(productId)).ReturnsAsync(product); //verilen id göre getasync methodu çağırıldığı zaman product değişkeni dönücek

            var result = await controller.Edit(productId); //Mvc tarafında edit methodu çağırılıyor

            var viewResult = Assert.IsType<ViewResult>(result); //Dönen tip viewResult mı kontrol ediliyor

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);//resultproduct Product tipinde referans verebilir mi kontrol ediliyor
            Assert.Equal(product.Id, resultProduct.Id);//Beklenen Id ile gerçekleşen ıd aynı mı kontrol ediliyor
            Assert.Equal(product.Name, resultProduct.Name);//Beklenen isim ile gerçekleşen isim aynı mı kontrol ediliyor

        }

        [Theory]
        [InlineData(1)] 
        public void EditPost_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            var result=controller.Edit(2,_products.First(x=>x.Id==productId));   //Burda verilen ıd ile product'da yer alan id örtüşüp örtüşmediğine bakıldı

            var redirect=Assert.IsType<NotFoundResult>(result);//Eğer id'ler örtüşmüyorsa NotFoundResult Dönüyor mu kontrol edildi.
        }

        [Theory]
        [InlineData(1)]

        public void EditPost_InvalidModelState_ReturnView(int productId)


        {
            controller.ModelState.AddModelError("Name", ""); //ModelState'de hata olması için hata fırlattık

            var result = controller.Edit(productId, _products.First(x => x.Id == productId));//ProductId ile product'tan gelen id aynı mı

            var viewResult= Assert.IsType<ViewResult>(result);  //Eğer modelstate hatalı ise viewResult dönüyor mu

            Assert.IsType<Product>(viewResult.Model);//Eğer viewResult dönüyorsa product nesnesi model olarak gönderiliyor mu
        }

        [Theory]
        [InlineData(1)] 

        public void EditPost_ValidModelState_ReturnRedirectToIndexAction(int productId)
        {
            var result= controller.Edit(productId,_products.First(x=>x.Id== productId));    //productId ile product'tan gelen id aynı mı test ediliyor

            var redirect=Assert.IsType<RedirectToActionResult>(result);//Dönüş tipi redirectActionResult dönüyor mu o kontrol ediliyor

            Assert.Equal("Index", redirect.ActionName);//Döndüğü yer Index mi o kontrol ediliyor
        }

        [Theory]
        [InlineData(1)] 

        public void EditPost_ValidModelState_UpdateMethodExecute(int productId)
        {
            var product = _products.First(x => x.Id == productId); //verilen id ile product değişkeni oluşturuldu

            _mockRepo.Setup(x => x.Update(product));//mockup nesnesi üzerinden setup işlemi yapıldı
             
            controller.Edit(productId, product); //Edit methodu için productId ve product parametreleri geçildi

            _mockRepo.Verify(x=>x.Update(It.IsAny<Product>()),Times.Once); //IRepository'den update çalışılıp çalışmadığı kontrol edildi
        }
    }
}
