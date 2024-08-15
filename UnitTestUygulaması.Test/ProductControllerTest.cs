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
    }
}
