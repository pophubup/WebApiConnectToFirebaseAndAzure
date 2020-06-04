using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Utility
{
    public class ProductQueryConditions
    {
        public QueryCondition<string> ProductID { get; set; }

        public QueryCondition<int> CategoryID { get; set; }

        public QueryCondition<string>ProductName { get; set; }
    }
}
