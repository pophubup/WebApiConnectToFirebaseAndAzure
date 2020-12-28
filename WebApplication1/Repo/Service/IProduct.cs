using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Repo.Service
{
    public interface IProduct
    {
        public IQueryable<Product> GetAllProducts();
        public List<Category> GetAllCategory();
        public IEnumerable<Product> GetProduct(Product Product);
        public List<Product> SaveNewProducts(List<NewProducts> product);
        public List<Product> CreateOrders(Order orders);
        public List<OrderProducts> GetOrderProducts();
        public bool SaveImageToBlobstroage(List<Product> products);
    }
}
