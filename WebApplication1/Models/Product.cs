using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    [FirestoreData]
    public class Product
    {
        [FirestoreProperty]
        public string ProductID { get; set; }
        [FirestoreProperty]
        public string ProductName { get; set; }
        [FirestoreProperty]
        public int CategoryID { get; set; }
        [FirestoreProperty]
        public double ProductPrice { get; set; }
        [FirestoreProperty]
        public string ProductDescription { get; set; }
        [FirestoreProperty]
        public string ProductImagePath { get; set; }
        [FirestoreProperty]
        public int Quantity { get; set; } = 1;
    }
}
