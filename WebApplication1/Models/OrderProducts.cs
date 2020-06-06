using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class OrderProducts
    {
        public string Date { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string Quantity { get; set; }
        public string Total { get; set; }
    }
}
