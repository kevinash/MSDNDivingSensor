// Kevin Ashley, Microsoft, 2018
// SensorKit
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using SensorKitSDK.Client.Common.Model;
using SensorKitSDK.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SensorKitSDK
{
    public class SessionDataManager : ISessionDataManager
    {
        public SessionDataContainer CreateSession()
        {
            var sdc = new SessionDataContainer();
            sdc.Session.Id = Guid.NewGuid().ToString();
            sdc.Session.Verb = "POST"; // Default is new add, can be overridden
            return sdc;
        }

        public async Task<CloudBlob> UploadSessionAsync(SessionDataContainer sessionDataContainer, CloudStorageAccount storageAccount, string storageContainerName)
        {
            return await new AzureUploader(storageAccount).UploadStringAsync(storageContainerName, $"{sessionDataContainer.Session.Id}.json",  Serialize.ToJson(sessionDataContainer));
        }

        public async Task<CloudBlob> UploadSensorDataFileAsync(SessionDataContainer sessionDataContainer, CloudStorageAccount storageAccount, string storageContainerName, string deviceId, string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            var blob = await new AzureUploader(storageAccount).UploadFileAsync(storageContainerName, fileName, filePath);

            blob.Metadata["device"] = JsonConvert.SerializeObject(sessionDataContainer.FindDevice(deviceId));
            await blob.SetMetadataAsync();

            return blob;
        }

        public string GetRawSensorDataFilename(SessionDataContainer sessionDataContainer, string deviceId)
        {
            return $"{sessionDataContainer.Session.Id}_{deviceId}.dat";
        }

        
    }
}
