using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.AspNetCore.Hosting;
using System.Drawing.Imaging;

namespace WebApplication1
{
    public class ImageCreator
    {
       
        public void Base64ToImage(string base64String,string rootpath)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                ms.Write(imageBytes, 0, imageBytes.Length);
                Image image = Image.FromStream(ms, true);
                Bitmap i2 = new Bitmap(image);
                image.Dispose();

                i2.Save(rootpath, ImageFormat.Jpeg);
                
            }
            catch(Exception ex)
            {

            }
          
            
        }
    }
}
