using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    [Serializable]
    public class MedaData
    {
        [JsonProperty("products")]
        List<NewProducts> products { get; set; }
    }
}
