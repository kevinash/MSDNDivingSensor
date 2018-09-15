using System.Collections.Generic;

using Newtonsoft.Json;

namespace SensorKitSDK.Client.Common.Model
{
    public class Labels
    {
        public Labels()
        {
            Properties = new Dictionary<string, string>();
        }

        [JsonProperty("uniquedeviceid")]
        public string UniqueDeviceId { get; set; }

        [JsonProperty("labelName")]
        public string LabelName { get; set; }

        [JsonProperty("labelValue")]
        public string LabelValue { get; set; }


        [JsonProperty("properties")]
        public Dictionary<string, string> Properties { get; set; }
    }
}