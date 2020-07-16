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

            var data = this.QueryConditions.GetType().GetProperties();
            foreach (var i in data)
            {

                if (i.Name == "ProductID" && !string.IsNullOrEmpty(this.QueryConditions.ProductID.Value))
                {
                    this.And(this.QueryConditions.ProductID, nameof(Product.ProductID));
                }
                else if (i.Name == "ProductName" && !string.IsNullOrEmpty(this.QueryConditions.ProductName.Value))
                {
                    this.And(this.QueryConditions.ProductName, nameof(Product.ProductName));
                }
                else if (i.Name == "CategoryID" && this.QueryConditions.CategoryID.Value != 0)
                {

                    this.And(this.QueryConditions.CategoryID, nameof(Product.CategoryID));

                }


            }

            return this.GenerateLambdaExpression();
        }
    }
}
