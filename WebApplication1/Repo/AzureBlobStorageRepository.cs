using Microsoft.Extensions.FileProviders;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Repo
{
    public class AzureBlobStorageRepository : ICloudClient<CloudBlobContainer, Product, Product>
    {
        private static PhysicalFileProvider _fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
        public CloudBlobContainer SetCleinttCredential 
        {
            get
            {
                IFileInfo fileInfo = _fileProvider.GetFileInfo("appsettings.json");
                Stream data = fileInfo.CreateReadStream();
                using (StreamReader stream = new StreamReader(data))
                {
                    string content = stream.ReadToEnd();
                    JToken jToken = JToken.Parse(content);
                    string key1 = (string)jToken.SelectToken("BlobStorageAccount.AccountName");
                    string key2 = (string)jToken.SelectToken("BlobStorageAccount.AccountKey");
                    StorageCredentials storageCredentials = new StorageCredentials(key1, key2);
                    CloudStorageAccount account = new CloudStorageAccount(storageCredentials, true);
                    CloudBlobClient serviceClient = account.CreateCloudBlobClient();
                    CloudBlobContainer container = serviceClient.GetContainerReference("products");

                    return container;
                }
            }
        
        }

        public Tuple<bool, string> ClientToDeleteData(Product viewModel)
        {
            throw new NotImplementedException();
        }

        public List<Product> ClientToGetData()
        {
           
            BlobContinuationToken continuationToken = null;
            BlobResultSegment resultSegment = SetCleinttCredential.ListBlobsSegmentedAsync(string.Empty, true, BlobListingDetails.Metadata, null, continuationToken, null, null).GetAwaiter().GetResult();
            List<Product> products = new List<Product>();
            
            resultSegment.Results.ToList().ForEach(x => {
                CloudBlob blob = (CloudBlob)x;
                products.Add(new Product() {
                     ProductName = blob.Name.Split('.')[0],
                     ProductImagePath = blob.Uri.ToString()

                });
            });
            return products;

        }

        public List<Product> ClientToGetDataByCondiction(Product viewModel)
        {
            throw new NotImplementedException();
        }

        public Product ClientToGetDataByUniqeID(string ID)
        {
            throw new NotImplementedException();
        }

        public Tuple<bool, Product> ClientToInsertData(Product viewModel)
        {

            CloudBlockBlob cloudBlockBlob = SetCleinttCredential.GetBlockBlobReference($"{viewModel.ProductName}{viewModel.FileType}");
            cloudBlockBlob.UploadFromFileAsync(viewModel.fileInfo.PhysicalPath);
            return new Tuple<bool, Product>(
                  true,
                  viewModel
                );
        }

        public Tuple<bool, string> ClientToUpdateData(Product viewModel)
        {
            throw new NotImplementedException();
        }
    }
}
