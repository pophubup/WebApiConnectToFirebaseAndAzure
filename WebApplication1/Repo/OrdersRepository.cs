using Google.Cloud.Firestore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Utility;

namespace WebApplication1.Repo
{
    public class OrdersRepository : IOrders
    {
        private static FirestoreDb db = CreateInstance.Instance.CreateDB;
        private static IMemoryCache _cache;
        public OrdersRepository(IMemoryCache cache)
        {
            _cache = cache;
        }
        public async Task<List<OrderDetails>> GetOrder()
        {
            List<OrderDetails> allorders = new List<OrderDetails>();
            //FireCloud
            if (!_cache.TryGetValue("AllOrders", out allorders))
            {
                Query allOrdersQuery = db.Collection("Orders");
                QuerySnapshot allCitiesQuerySnapshot = await allOrdersQuery.GetSnapshotAsync();
                List<OrderDetails> orderDetails = new List<OrderDetails>();
                Task<List<OrderDetails>> data = Task.Run(() =>
                {
                    foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
                    {
                        Dictionary<string, object> order = documentSnapshot.ToDictionary();
                        string json = JsonConvert.SerializeObject(order);
                         OrderDetails data = JsonConvert.DeserializeObject<OrderDetails>(json);
                        data.DateID = documentSnapshot.Id;
                        orderDetails.Add(data);
                    }
                    return orderDetails;
                });
                _cache.Set("AllOrders", orderDetails, TimeSpan.FromSeconds(600));

                return await data;
            }
            else
            {
                allorders = _cache.Get<List<OrderDetails>>("AllOrders");
                return allorders;
            }
        }
    }
}
