//standard namespaces
using System.IO;

//imported .dll's
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

using RestSharp;
using RestSharp.Deserializers;

namespace TwitchLibrary.Helpers.Json
{
    internal class CustomBsonDeserializer : IDeserializer
    {
        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }

        /// <summary>
        /// Custom deserializer that utilizies Newtonsoft to handle Bson responses with RestSharp
        /// </summary>
        public type Deserialize<type>(IRestResponse response)
        {
            using (MemoryStream memory_stream = new MemoryStream())
            {
                using (BsonReader bson_reader = new BsonReader(memory_stream))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    return serializer.Deserialize<type>(bson_reader);
                }
            }
        }
    }
}
