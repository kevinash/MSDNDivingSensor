namespace SensorKitSDK.Client.Common.Model
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public partial class Device
    {
        public Device()
        {
            Properties = new Dictionary<string, string>();
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string DeviceType { get; set; }
            
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, string> Properties { get; set; }

        [JsonProperty("sessionid")]
        public string SessionId { get; set; }
    }
}