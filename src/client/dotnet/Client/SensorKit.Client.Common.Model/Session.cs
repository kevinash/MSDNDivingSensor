using System.Linq;

namespace SensorKitSDK.Client.Common.Model
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public partial class Session
    {
        public Session()
        {
            Properties = new Dictionary<string, string>();
            Devices = new List<Device>();
            
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("startTime")]
        public System.DateTime StartTime { get; set; }

        [JsonProperty("endTime")]
        public System.DateTime EndTime { get; set; }

        [JsonProperty("verb")]
        public string Verb { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, string> Properties { get; set; }

        [JsonProperty("devices")]
        public List<Device> Devices { get; set; }

        [JsonProperty("labels")]
        public List<Labels> Labels { get; set; }
    }
}