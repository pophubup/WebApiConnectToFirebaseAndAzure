using Google.Cloud.Firestore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    [FirestoreData]
    public class OrderDetails
    {
        [FirestoreProperty]
        public string DateID { get; set; }
        [JsonProperty("ProductID")]
        [FirestoreProperty]
        public IList<string> ProductID { get; set; }
        [JsonProperty("Quantity")]
        [FirestoreProperty]
        public IList<string> Quantity { get; set; }
        [JsonProperty("Total")]
        [FirestoreProperty]
        public IList<string> Total { get; set; }
    }
}


