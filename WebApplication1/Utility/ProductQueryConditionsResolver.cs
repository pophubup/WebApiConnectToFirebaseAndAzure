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
  
            if ( !string.IsNullOrEmpty(this.QueryConditions.ProductID.Value) && 
                 this.QueryConditions.CategoryID.Value == 0 && 
                 string.IsNullOrEmpty( this.QueryConditions.ProductName.Value))
            {
                this.And(this.QueryConditions.ProductID, nameof(Product.ProductID));

            }
            else if (string.IsNullOrEmpty(this.QueryConditions.ProductID.Value) && 
                     this.QueryConditions.CategoryID.Value != 0 && 
                     string.IsNullOrEmpty(this.QueryConditions.ProductName.Value))
            {
                this.And(this.QueryConditions.CategoryID, nameof(Product.CategoryID));
            }
            else if (string.IsNullOrEmpty(this.QueryConditions.ProductID.Value) && 
                     this.QueryConditions.CategoryID.Value == 0 && 
                     !string.IsNullOrEmpty(this.QueryConditions.ProductName.Value))
            {
                this.And(this.QueryConditions.ProductName, nameof(Product.ProductName));
            }
            else if (!string.IsNullOrEmpty(this.QueryConditions.ProductID.Value) &&  this.QueryConditions.CategoryID.Value != 0 && !string.IsNullOrEmpty(this.QueryConditions.ProductName.Value))
            {
                this.And(this.QueryConditions.ProductID, nameof(Product.ProductID));
                this.And(this.QueryConditions.CategoryID, nameof(Product.CategoryID));
            }
            else if (!string.IsNullOrEmpty(this.QueryConditions.ProductID.Value) && !string.IsNullOrEmpty(this.QueryConditions.ProductName.Value) && this.QueryConditions.CategoryID.Value == 0)
            {
                this.And(this.QueryConditions.ProductID, nameof(Product.ProductID));
                this.And(this.QueryConditions.ProductName, nameof(Product.ProductName));
            }
            else if (this.QueryConditions.CategoryID.Value != 0 && !string.IsNullOrEmpty(this.QueryConditions.ProductName.Value) && string.IsNullOrEmpty(this.QueryConditions.ProductID.Value))
            {
                this.And(this.QueryConditions.CategoryID, nameof(Product.CategoryID));
                this.And(this.QueryConditions.ProductName, nameof(Product.ProductName));
            }
            else if (!string.IsNullOrEmpty(this.QueryConditions.ProductName.Value) && !string.IsNullOrEmpty(this.QueryConditions.ProductID.Value)  && this.QueryConditions.CategoryID.Value != 0)
            {
                this.And(this.QueryConditions.ProductID, nameof(Product.ProductID));
                this.And(this.QueryConditions.CategoryID, nameof(Product.CategoryID));
                this.And(this.QueryConditions.ProductName, nameof(Product.ProductName));
            }

                return this.GenerateLambdaExpression();
        }
    }
}
