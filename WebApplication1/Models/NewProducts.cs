using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    [Serializable]
    public class NewProducts
    {

        [JsonProperty("ProductID")]
        public string ProductID { get; set; }
        [JsonProperty("ProductName")]
        public string ProductName { get; set; }
        [JsonProperty("CategoryID")]
        public string CategoryID { get; set; }
        [JsonProperty("ProductPrice")]
        public string ProductPrice { get; set; }
        [JsonProperty("ProductDescription")]
        public string ProductDescription { get; set; }
        [JsonProperty("ProductImagePath")]
        public string ProductImagePath { get; set; }
        [JsonProperty("Quantity")]
        public string Quantity { get; set; }
    }
}
