using System;
using System.Linq;
using Newtonsoft.Json;


namespace SensorKitSDK.Client.Common.Model
{
    public partial class SessionDataContainer
    {
        public SessionDataContainer()
        {
            Session = new Session();
        }
        [JsonProperty("schemaversion")]
        public string SchemaVersion { get; set; }

        [JsonProperty("Session")]
        public Session Session { get; set; }

        public static SessionDataContainer FromJson(string json) => JsonConvert.DeserializeObject<SessionDataContainer>(json, Converter.Settings);

        public Device FindDevice(string deviceid)
        {
            foreach (var device in Session.Devices)
            {
                if (device.Id == deviceid)
                {
                    return device;
                }
            }

            // not found 
            return null;
        }

        
    }
}
