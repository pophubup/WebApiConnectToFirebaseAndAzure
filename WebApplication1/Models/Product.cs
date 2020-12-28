using Google.Cloud.Firestore;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Utility;

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

        public string FileType { 
            get
            {
                return string.IsNullOrEmpty(ProductImagePath) ? string.Empty : $".{ProductImagePath.Split(',')[0].Split('/')[1]}";
            } 
        }
        public IFileInfo fileInfo { 
            get 
            {
                if(ProductName != null)
                {
                    return Base64ToImage.ConvertImagetoByteArray($"{ProductName}{FileType}");
                }
                else
                {
                    return null;
                }
                
            } 
        }
    }
}
