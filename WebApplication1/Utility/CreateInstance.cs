using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Linq;

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
                string key1 = configuration["BlobStorageAccount:AccountName"];
                string key2 = configuration["BlobStorageAccount:AccountKey"];
                StorageCredentials storageCredentials = new StorageCredentials(key1, key2);
                CloudStorageAccount account = new CloudStorageAccount(storageCredentials, true);
                CloudBlobClient serviceClient = account.CreateCloudBlobClient();
                CloudBlobContainer container = serviceClient.GetContainerReference("products");
                return container;
            }
        }
     
    }
}
