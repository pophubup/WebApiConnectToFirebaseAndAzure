using Google.Cloud.Firestore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Utility;

namespace WebApplication1.Repo.Service
{
    public class OrderService : IOrder
    {
  
        private ICloudClient<FirestoreDb, Order, OrderDetails> _FirstStoreOrder;
        private static IMemoryCache _cache;
        public OrderService(IMemoryCache cache,  ICloudClient<FirestoreDb, Order, OrderDetails> FirstStoreOrder)
        {
            _cache = cache;
            _FirstStoreOrder = FirstStoreOrder;
        }
        public List<OrderDetails> GetOrder()
        {
            List<OrderDetails> allorders = new List<OrderDetails>();
            if (!_cache.TryGetValue("AllOrders", out allorders))
            {
                var orderDetails = _FirstStoreOrder.ClientToGetData();
                _cache.Set("AllOrders", orderDetails, TimeSpan.FromSeconds(600));

                return orderDetails;
            }
            else
            {
                allorders = _cache.Get<List<OrderDetails>>("AllOrders");
                return allorders;
            }
        }
    }
}
