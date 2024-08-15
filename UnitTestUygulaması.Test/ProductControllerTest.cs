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
    }
}
