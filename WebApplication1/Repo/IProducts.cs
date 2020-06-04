using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Repo
{
    public interface IProducts
    {
        public Task<IQueryable<Product>> GetAllProducts();
        public List<Category> GetAllCategory();
        public Task<IEnumerable<Product>> GetProduct(Product Product);
        public Task<List<Product>> SaveNewProducts(List<NewProducts> product);
        public Task<List<Product>> CreateOrders(Order orders);
        public Task<List<OrderProducts>> GetOrderProducts();
        public Task<bool> SaveImageToBlobstroage(List<Product> products);
    }
}
