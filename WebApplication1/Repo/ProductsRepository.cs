using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Utility;

namespace WebApplication1.Repo
{
    public class ProductsRepository : IProducts
    {
        private static  FirestoreDb db = CreateInstance.Instance.CreateDB;
        private static IMemoryCache _cache;
        private static IOrders _orders;
        private IWebHostEnvironment _hostingEnvironment;
        public ProductsRepository(IMemoryCache cache, IWebHostEnvironment hostingEnvironment, IOrders orders)
        {
            _cache = cache;
            _orders = orders;
            _hostingEnvironment = hostingEnvironment;
        }
        public List<Category> GetAllCategory()
        {
            return Categories.GetCategoryID();
        }
        public async Task<IQueryable<Product>> GetAllProducts()
        {
            IQueryable<Product> allproducts;
            //FireCloud
            if (!_cache.TryGetValue("AllProducts", out allproducts))
            {
                allproducts = CreateInstance.Instance.GetProducts().AsQueryable();
                _cache.Set("AllProducts", allproducts, TimeSpan.FromSeconds(600));
                return allproducts;
            }
            else
            {
                int countProducts = db.Collection("Products").GetSnapshotAsync().GetAwaiter().GetResult().Count;
              
                if(allproducts.Count() == countProducts)
                {
                    allproducts = _cache.Get<IQueryable<Product>>("AllProducts");
                }
                else
                {
                   
                    allproducts = CreateInstance.Instance.GetProducts();
                    _cache.Set("AllProducts", allproducts, TimeSpan.FromSeconds(600));
                }
                
                return allproducts;
            }
        }
        public async Task<IEnumerable<Product>> GetProduct(Product Product)
        {        
            IQueryable<Product> datalist = await GetAllProducts();

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
        public async Task<List<Product>> SaveNewProducts(List<NewProducts> products)
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
                        string combineIndex = "abcdefghlijkmnopqr";
                        for (int j = 0; j < 3; j++)
                        {
                            int procnumber = random.Next(0, 6);
                            procid += procnumber.ToString() + combineIndex[random.Next(0, 6)].ToString() + combineIndex[random.Next(0, 6)].ToString();
                        }
                        i.ProductID = procid;
                        DocumentReference addedDocRef = db.Collection("Products").Document(i.ProductName);
                        await addedDocRef.CreateAsync(i);
                        i.ProductImagePath = products.FirstOrDefault(x=>x.ProductName == i.ProductName).ProductImagePath;
                        keeper.Add(i);
                    
                    

                }
             
          
            return keeper;
        }
        public async Task<bool> SaveImageToBlobstroage(List<Product> products)
        {
            string subfolder = @"\Images";
            string path = _hostingEnvironment.ContentRootPath + subfolder;
            //string path = @"D:\home\site\wwwroot\images";
            List<string> result = Base64ToImage.runMutipleData(products, path);
            int countsave = 0;
            foreach (Product i in products)
            {
                string checktype = i.ProductImagePath.Split(',')[0].Split('/')[1];
                CloudBlobContainer container = CreateInstance.Instance.cloudBlobContainer;
                IFileInfo file;
                if (checktype.Contains("png"))
                {
                    file = Base64ToImage.ConvertImagetoByteArray(i.ProductName + ".png");
                    countsave++;
                }
                else
                {
                    file = Base64ToImage.ConvertImagetoByteArray(i.ProductName + ".jpg");
                    countsave++;
                }

                if (file.Name.Contains("png"))
                {
                    CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(i.ProductName + ".png");
                    await cloudBlockBlob.UploadFromFileAsync(file.PhysicalPath);
                }
                else
                {
                    CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(i.ProductName + ".jpg");
                    await cloudBlockBlob.UploadFromFileAsync(file.PhysicalPath);
                }
            }
            return countsave == products.Count ? true : false;
            
        }
        public async Task<List<Product>> CreateOrders(Order orders)
        {
            
            DocumentReference docRef = db.Collection("Orders").Document(orders.OrderID);
             await docRef.CreateAsync(orders.orderDetails);
            IQueryable<Product> products = await GetAllProducts();
            List<Product> datalist = new List<Product>();
            foreach (var i in orders.orderDetails.ProductID)
            {
                Product prod = products.FirstOrDefault(x => x.ProductID == i);
                datalist.Add(prod);
            }
            return datalist;
        }
        public async Task<List<OrderProducts>> GetOrderProducts()
        {
            List<OrderDetails> orderDetails = await _orders.GetOrder();  
            IQueryable<Product> products = await GetAllProducts();
            List<OrderProducts> orderProducts = new List<OrderProducts>();
            Task<List<OrderProducts>> data = Task.Run(() =>
            {
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
            });
           
            return await data;
        }

    }
}
