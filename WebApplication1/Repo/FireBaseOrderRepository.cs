using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Repo
{
    public class FireBaseOrderRepository : ICloudClient<FirestoreDb, Order, OrderDetails>
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

        public Tuple<bool, string> ClientToDeleteData(Order viewModel)
        {
            throw new NotImplementedException();
        }

        public List<OrderDetails> ClientToGetData()
        {
            List<OrderDetails> allorders = new List<OrderDetails>();
            //FireCloud
            
                Query allOrdersQuery = SetCleinttCredential.Collection("Orders");
                QuerySnapshot allCitiesQuerySnapshot =  allOrdersQuery.GetSnapshotAsync().GetAwaiter().GetResult();
                List<OrderDetails> orderDetails = new List<OrderDetails>();

                    foreach (DocumentSnapshot documentSnapshot in allCitiesQuerySnapshot.Documents)
                    {
                        Dictionary<string, object> order = documentSnapshot.ToDictionary();
                        string json = JsonConvert.SerializeObject(order);
                        OrderDetails data = JsonConvert.DeserializeObject<OrderDetails>(json);
                        data.DateID = documentSnapshot.Id;
                        orderDetails.Add(data);
                    }
             
                
              

                return orderDetails;
           
        }

        public List<OrderDetails> ClientToGetDataByCondiction(Order viewModel)
        {
            throw new NotImplementedException();
        }

        public OrderDetails ClientToGetDataByUniqeID(string ID)
        {
            throw new NotImplementedException();
        }

        public Tuple<bool, OrderDetails> ClientToInsertData(Order viewModel)
        {

            DocumentReference docRef = SetCleinttCredential.Collection("Orders").Document(viewModel.OrderID);
             docRef.CreateAsync(viewModel.orderDetails).GetAwaiter().GetResult();
           
        
            return new Tuple<bool, OrderDetails>(
                item1 : true,
                item2 : viewModel.orderDetails
                );
        }

        public Tuple<bool, string> ClientToUpdateData(Order viewModel)
        {
            throw new NotImplementedException();
        }
    }
}
