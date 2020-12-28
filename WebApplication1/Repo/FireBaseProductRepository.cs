using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Repo
{
    public class FireBaseProductRepository : ICloudClient<FirestoreDb, Product, Product>
    {
        private static PhysicalFileProvider _fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
        public FirestoreDb SetCleinttCredential 
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

        public Tuple<bool, string> ClientToDeleteData(Product viewModel)
        {
            throw new NotImplementedException();
        }

        public List<Product> ClientToGetData()
        {
            List<Product> products = new List<Product>();
            Query allCitiesQuery = SetCleinttCredential.Collection("Products");
            QuerySnapshot allCitiesQuerySnapshot = allCitiesQuery.GetSnapshotAsync().GetAwaiter().GetResult();

            foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
            {
                Product product = documentSnapshot.ConvertTo<Product>();
                products.Add(product);
            }
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

            string procid = string.Empty;
            Random random = new Random();

            string combineIndex = Guid.NewGuid().ToString();
            for (int j = 0; j < 3; j++)
            {
                int procnumber = random.Next(0, 6);
                procid += procnumber.ToString() + combineIndex[random.Next(0, 6)].ToString() + combineIndex[random.Next(0, 6)].ToString();
            }
            viewModel.ProductID = procid;
            DocumentReference addedDocRef = SetCleinttCredential.Collection("Products").Document(viewModel.ProductName);
           var afterInsert =  addedDocRef.CreateAsync(viewModel).GetAwaiter().GetResult();

            return new Tuple<bool, Product>(
                  item1 : true,
                  item2:viewModel
                );
        }

        public Tuple<bool, string> ClientToUpdateData(Product viewModel)
        {
            throw new NotImplementedException();
        }
    }
}
