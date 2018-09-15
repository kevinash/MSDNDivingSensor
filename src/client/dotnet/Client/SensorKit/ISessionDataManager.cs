// Kevin Ashley, Microsoft, 2018
// SensorKit
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SensorKitSDK.Client.Common.Model;

namespace SensorKitSDK
{
    public interface ISessionDataManager
    {
        SessionDataContainer CreateSession();
        Task<CloudBlob> UploadSensorDataFileAsync(SessionDataContainer sessionDataContainer, CloudStorageAccount storageAccount, string storageContainerName, string deviceId, string filePath);
        Task<CloudBlob> UploadSessionAsync(SessionDataContainer sessionDataContainer, CloudStorageAccount storageAccount, string storageContainerName);
        string GetRawSensorDataFilename(SessionDataContainer sessionDataContainer, string deviceId);
    }
}