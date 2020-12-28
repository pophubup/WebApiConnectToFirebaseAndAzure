using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Models;
using WebApplication1.Utility;

namespace WebApplication1.Repo.Service
{
    public class ProductService : IProduct
    {
        private IMemoryCache _cache;
        private IOrder _orders;
        private ICloudClient<FirestoreDb, Product, Product> _FireBaseCleint;
        private ICloudClient<CloudBlobContainer, Product, Product> _AzureBlobClient;
        private ICloudClient<FirestoreDb, Order, OrderDetails> _FirstStoreOrder;
        private IWebHostEnvironment _hostingEnvironment;
        public ProductService(IMemoryCache cache,
                                  IWebHostEnvironment hostingEnvironment, 
                                  IOrder orders, 
                                  ICloudClient<FirestoreDb, Product, Product> FireBaseCleint, 
                                  ICloudClient<CloudBlobContainer, Product, Product> AzureBlobClient,
                                  ICloudClient<FirestoreDb, Order, OrderDetails> FirstStoreOrder)
        {
            _cache = cache;
            _orders = orders;
            _hostingEnvironment = hostingEnvironment;
            _FireBaseCleint = FireBaseCleint;
            _AzureBlobClient = AzureBlobClient;
            _FirstStoreOrder = FirstStoreOrder;
        }
        public List<Category> GetAllCategory()
        {
            return Categories.GetCategoryID();
        }
        public IQueryable<Product> GetAllProducts()
        {
            IQueryable<Product> allproducts;
            if (!_cache.TryGetValue("AllProducts", out allproducts))
            {
                allproducts = _FireBaseCleint.ClientToGetData().AsQueryable();
                var Product_Pics = _AzureBlobClient.ClientToGetData();
                foreach (var pic in Product_Pics)
                {
                    foreach (var i in allproducts)
                    {
                        if (i.ProductName == pic.ProductName)
                        {
                            i.ProductImagePath = pic.ProductImagePath;
                        }
                    }

                }
                _cache.Set("AllProducts", allproducts, TimeSpan.FromSeconds(600));
                return allproducts;
            }
            else
            {
                int countProducts = _FireBaseCleint.ClientToGetData().Count;
              
                if(allproducts.Count() == countProducts)
                {
                    allproducts = _cache.Get<IQueryable<Product>>("AllProducts");
                }
                else
                {
                   
                    allproducts = _FireBaseCleint.ClientToGetData().AsQueryable();
                    _cache.Set("AllProducts", allproducts, TimeSpan.FromSeconds(600));
                }
                
                return allproducts;
            }
        }
        public IEnumerable<Product> GetProduct(Product Product)
        {
            IQueryable<Product> datalist = GetAllProducts();
            ProductQueryConditions productQueryConditions = new ProductQueryConditions()
            {
                ProductID = new QueryCondition<string>(QueryComparsion.Equal, Product.ProductID != null ? Product.ProductID : null),
                CategoryID = new QueryCondition<int>(QueryComparsion.Equal,Product.CategoryID != 0 ?  Product.CategoryID : 0 ),
                ProductName = new QueryCondition<string>(QueryComparsion.StartsWith, Product.ProductName != null ? Product.ProductName : null)
            };
            ProductQueryConditionsResolver productQueryConditionsResolver = new ProductQueryConditionsResolver(productQueryConditions);
            IEnumerable<Product> result = datalist.Where(productQueryConditionsResolver.Resolve()).AsEnumerable();
            return result;
        }
        public List<Product> SaveNewProducts(List<NewProducts> products)
        {
            List<Product> datalist = new List<Product>();
            List<Product> keeper = new List<Product>();
           
                foreach (NewProducts i in products)
                {
                    Product _model = new Product()
                    {
                        CategoryID = Convert.ToInt32(i.CategoryID),
                        ProductDescription = i.ProductDescription.Trim(),
                        ProductID = i.ProductID,
                        ProductImagePath = i.ProductName.Trim() + ".png",
                        ProductName = i.ProductName.Trim(),
                        ProductPrice = Convert.ToDouble(i.ProductPrice),
                        Quantity = Convert.ToInt32(i.Quantity)

                    };
                    datalist.Add(_model);
                }
          
                foreach (Product i in datalist)
                {
                   
                        string procid = string.Empty;
                        Random random = new Random();
                    
                        string combineIndex = Guid.NewGuid().ToString();
                        for (int j = 0; j < 3; j++)
                        {
                            int procnumber = random.Next(0, 6);
                            procid += procnumber.ToString() + combineIndex[random.Next(0, 6)].ToString() + combineIndex[random.Next(0, 6)].ToString();
                        }
                        i.ProductID = procid;
                        i.ProductImagePath = products.FirstOrDefault(x => x.ProductName == i.ProductName).ProductImagePath;
                         _FireBaseCleint.ClientToInsertData(i);
                        keeper.Add(i);
                }
            return keeper;
        }
        public bool SaveImageToBlobstroage(List<Product> products)
        {
            
            string path = $"{_hostingEnvironment.ContentRootPath}\\Images";
            List<string> result = Base64ToImage.runMutipleData(products, path);
            foreach (Product i in products)
            {
               _AzureBlobClient.ClientToInsertData(i);
            }
            return true ;
            
        }
        public List<Product> CreateOrders(Order order)
        {
            _FirstStoreOrder.ClientToInsertData(order);
            IQueryable<Product> products = GetAllProducts();
            List<Product> datalist = new List<Product>();
            var result = _FirstStoreOrder.ClientToInsertData(order);
            foreach (var i in result.Item2.ProductID)
            {
                Product prod = products.FirstOrDefault(x => x.ProductID == i);
                datalist.Add(prod);
            }
            return datalist;
        }
        public List<OrderProducts> GetOrderProducts()
        {
            List<OrderDetails> orderDetails = _FirstStoreOrder.ClientToGetData();  
            IQueryable<Product> products = GetAllProducts();
            List<OrderProducts> orderProducts = new List<OrderProducts>();
          
                for (int i = 0; i < orderDetails.Count; i++)
                {
                    for (int j = 0; j < orderDetails[i].ProductID.Count; j++)
                    {
                        OrderProducts _model = new OrderProducts();
                        _model.Date = orderDetails[i].DateID;
                        _model.ProductID = orderDetails[i].ProductID[j];
                        _model.Quantity = orderDetails[i].Quantity[j];
                        _model.Total = orderDetails[i].Total[j];
                        _model.ProductName = products.FirstOrDefault(x => x.ProductID == orderDetails[i].ProductID[j]).ProductName;
                        orderProducts.Add(_model);
                    }

                }
            return orderProducts;
        }

    }
}
