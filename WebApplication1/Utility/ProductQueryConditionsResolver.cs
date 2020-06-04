using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Utility
{
    public class ProductQueryConditionsResolver : QueryConditionsResolver<Product, ProductQueryConditions>
    {
        public ProductQueryConditionsResolver(ProductQueryConditions productQueryConditions)
           : base(productQueryConditions)
        {
        }
        public override Expression<Func<Product, bool>> Resolve()
        {
            this.And(this.QueryConditions.ProductID, nameof(Product.ProductID));
            return this.GenerateLambdaExpression();
        }
    }
}
