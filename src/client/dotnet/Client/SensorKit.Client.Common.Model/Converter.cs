namespace SensorKitSDK.Client.Common
{
    using Newtonsoft.Json;

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings 
            = new JsonSerializerSettings
            {
                //TypeNameHandling = TypeNameHandling.All,
                //TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                Formatting = Formatting.Indented
            };
    }
}