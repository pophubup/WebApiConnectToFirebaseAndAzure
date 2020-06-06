using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Category
    {       
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }

    }
    public static class Categories
    {
        public static List<Category> GetCategoryID()
        {
            List<Category> categories = new List<Category>()
            {
                 new Category{CategoryID = 0, CategoryName ="Uncategorized"},
                new Category{CategoryID = 1, CategoryName ="Liquid"},
                new Category{CategoryID = 2, CategoryName ="Production"},
                new Category{CategoryID = 3, CategoryName ="Meat"}
            };
            return categories;
        }
        
    }
}
