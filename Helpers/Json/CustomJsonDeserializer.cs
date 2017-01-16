//imported .dll's
using Newtonsoft.Json;

using RestSharp;
using RestSharp.Deserializers;

namespace TwitchLibrary.Helpers.Json
{
    class CustomJsonDeserializer : IDeserializer
    {
        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }        

        /// <summary>
        /// Cusatom deserializer to handle null values and automatically convert all date values to locla time.
        /// </summary>
        public type Deserialize<type>(IRestResponse response)
        {            
            return JsonConvert.DeserializeObject<type>(response.Content, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Local  });
        }
    }
}
