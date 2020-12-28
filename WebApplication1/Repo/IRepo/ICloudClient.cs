using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Repo
{
    public interface ICloudClient<Client,Model,ViewModel> where Client : class
                                                          where Model :class
                                                          where ViewModel : class
    {
        Client SetCleinttCredential { get; }
        List<ViewModel> ClientToGetData();
        List<ViewModel> ClientToGetDataByCondiction(Model viewModel);
        ViewModel ClientToGetDataByUniqeID(string ID);
        Tuple<bool, string> ClientToDeleteData(Model viewModel);
        Tuple<bool, string> ClientToUpdateData(Model viewModel);
        Tuple<bool, ViewModel>  ClientToInsertData(Model viewModel);
    }
}
