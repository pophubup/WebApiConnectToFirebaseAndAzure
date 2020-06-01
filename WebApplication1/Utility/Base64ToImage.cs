using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using WebApplication1.Models;

namespace WebApplication1.Utility
{
    public static class Base64ToImage
    {
        private static PhysicalFileProvider _fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
  
        public static List<string> runMutipleData(List<Product> products, string path)
        {
            List<string> checkreuslt = new List<string>();
            foreach(Product i in products)
            {
                string result = Base64ToImage.ConvertBase64ToImage(i.ProductImagePath, path, i.ProductName);
                if (result != "success")
                {
                    checkreuslt.Add(result);
                }
            }
            return checkreuslt;
        }
        public static IFileInfo ConvertImagetoByteArray(string filename)
        {
            IDirectoryContents folder = _fileProvider.GetDirectoryContents(@"\Images\");
            IEnumerator<IFileInfo> pathlist = folder.GetEnumerator();
            List<IFileInfo> imageList = ConvertEnumerator(pathlist);
            IFileInfo obj = imageList.Find(x => x.Name == filename);
            return obj;
        }
        private static List<IFileInfo> ConvertEnumerator (IEnumerator<IFileInfo> e)
        {
            List<IFileInfo> list = new List<IFileInfo>();
            while (e.MoveNext())
            {
                list.Add(e.Current);
            }
            return list;
        }
        private static string ConvertBase64ToImage(string base64String, string path, string ImageName)
        {
            string[] pd = base64String.Split(',');
            string[] checkformat = pd[0].Split('/');
            byte[] imageBytes = Convert.FromBase64String(pd[1]);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            string result = string.Empty;
            if (checkformat[1].Split(';')[0] == "png")
            {
                result = Base64ToImage.SaveResize_PNG_Tofolder(image, path, ImageName);
            }
            else
            {
                result = Base64ToImage.SaveResize_JPG_Tofolder(image, path, ImageName);
            }
           
            return result;
        }
        private static string SaveResize_JPG_Tofolder(Image img, string path, string ImageName)
        {

            Bitmap result = new Bitmap(img, 300, 300);
            result.SetResolution(3024, 4032);

            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.DrawImage(result, 0, 0, result.Width, result.Height);
            }

            try
            {
                result.Save($"{path}\\{ImageName}.jpg", ImageFormat.Jpeg);
                return "success";
            }
            catch (Exception ex)
            {
                return ImageName;
            }
        }
        private static string SaveResize_PNG_Tofolder(Image img, string path, string ImageName)
        {

                Bitmap result = new Bitmap(img, 300, 300);
                result.SetResolution(3024, 4032);

                using (Graphics graphics = Graphics.FromImage(result))
                {
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphics.DrawImage(result, 0, 0, result.Width, result.Height);
                }

                try
                {
                    result.Save($"{path}\\{ImageName}.png", ImageFormat.Png);
                    return "success";
                }
                catch (Exception ex)
                {
                    return ImageName;
                }
        }
    }
}
