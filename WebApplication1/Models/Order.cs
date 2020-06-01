using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    [FirestoreData]
    public class Order
    {
        [FirestoreProperty]
        public string OrderID { get; set; }
        [FirestoreProperty]
        public OrderDetails orderDetails { get; set; }
    }
}
