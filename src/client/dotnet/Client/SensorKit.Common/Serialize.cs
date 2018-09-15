using System;
using Newtonsoft.Json;

namespace SensorKitSDK.Common
{
    public static class Serialize
    {
        public static T ToObject<T>(string json) => JsonConvert.DeserializeObject<T>(json, Converter.Settings);
        public static string ToJson<T>(this T self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }
}
