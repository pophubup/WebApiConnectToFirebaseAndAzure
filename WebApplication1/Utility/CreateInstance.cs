using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebApplication1.Models;

namespace WebApplication1.Utility
{
    public class CreateInstance
    {
        
        private static PhysicalFileProvider _fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
        private static IConfiguration configuration = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
             .AddEnvironmentVariables()
             .Build();
        private static CreateInstance instance = null;
        private static readonly object padlock = new object();
        private CreateInstance()
        {
        }
        public static CreateInstance Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new CreateInstance();
                    }
                    return instance;
                }
            }
        }
        public FirestoreDb CreateDB
        {
            get
            {
                IDirectoryContents folder = _fileProvider.GetDirectoryContents(@"\FireBaseApikey\");
                GoogleCredential cred = GoogleCredential.FromFile(folder.FirstOrDefault().PhysicalPath);
                Grpc.Core.Channel channel = new Grpc.Core.Channel(FirestoreClient.DefaultEndpoint.Host,
                             FirestoreClient.DefaultEndpoint.Port,
                               cred.ToChannelCredentials());
                FirestoreClient client = FirestoreClient.Create(channel);
                FirestoreDb db = FirestoreDb.Create("getproducts-92bee", client);
                return db;
            }
        }

        public CloudBlobContainer cloudBlobContainer
        {
            get
            {
                IFileInfo fileInfo = _fileProvider.GetFileInfo("appsettings.json");
                string key1 = configuration["BlobStorageAccount:AccountName"];
                string key2 = configuration["BlobStorageAccount:AccountKey"];
                StorageCredentials storageCredentials = new StorageCredentials(key1, key2);
                CloudStorageAccount account = new CloudStorageAccount(storageCredentials, true);
                CloudBlobClient serviceClient = account.CreateCloudBlobClient();
                CloudBlobContainer container = serviceClient.GetContainerReference("products");
             
                return container;
            }
        }
        public IQueryable<Product> GetProducts()
        {
            
                List<Product> products = new List<Product>();
                Query allCitiesQuery = CreateDB.Collection("Products");
                QuerySnapshot allCitiesQuerySnapshot = allCitiesQuery.GetSnapshotAsync().GetAwaiter().GetResult();

                foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
                {
                    Product product = documentSnapshot.ConvertTo<Product>();
                    products.Add(product);
                }
                //FireCloud
                //Azure
                CloudBlobContainer container = CreateInstance.Instance.cloudBlobContainer;
                BlobContinuationToken continuationToken = null;
                CloudBlob blob;
                BlobResultSegment resultSegment =  container.ListBlobsSegmentedAsync(string.Empty, true, BlobListingDetails.Metadata, null, continuationToken, null, null).GetAwaiter().GetResult();
                foreach (var blobItem in resultSegment.Results)
                {
                    // A flat listing operation returns only blobs, not virtual directories.
                    blob = (CloudBlob)blobItem;
                    foreach (var i in products)
                    {
                        if (i.ProductName == blob.Name.Split('.')[0])
                        {
                            i.ProductImagePath = blob.Uri.ToString();
                        }
                    }

                }
            //Azure
            return products.AsQueryable() ;
            
        }

    }
}
