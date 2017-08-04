// imported .dll's
using Newtonsoft.Json;

using RestSharp;
using RestSharp.Deserializers;

namespace TwitchLibrary.Helpers.Json
{
    internal class CustomJsonDeserializer : IDeserializer
    {
        public string RootElement   { get; set; }
        public string Namespace     { get; set; }
        public string DateFormat    { get; set; }

        /// <summary>
        /// Custom deserializer that utilizies Newtonsoft to handle Json responses with RestSharp
        /// </summary>
        public type Deserialize<type>(IRestResponse response)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local;

            return JsonConvert.DeserializeObject<type>(response.Content, settings);
        }
    }
}
