using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class BaseModel
    {
        public string isWebApi { 
            get 
            { 
                 if(Controller == "Default")
                {
                    return "api";
                }
                else
                {
                    return null;
                }
            } 
        }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Attribute { get; set; }
        public string ReturnType { get; set; }
        public string Description 
        { 
            get 
            {
                switch (Controller)
                {
                    case "Default":
                        string result = string.Empty;
                        if(Attribute == "HttpPost")
                        {
                            result = "Post data with object";
                        }
                        else
                        {
                            result =  "retrieve data";
                        }
                    return result;
                    case "Home":
                        return "Main Page";
                    case "LineBot":
                        return "Webapi with Linbot";
                    default:
                        return null;
                        
                }
            } 
        }

    }
}
