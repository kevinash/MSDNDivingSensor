// Kevin Ashley, Microsoft, 2018
// SensorKit
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorKitSDK
{
    public class AzureUploader
    {
        CloudBlobClient blobClient = null;

        public AzureUploader(CloudStorageAccount account)
        {
            // Create the blob client.
            blobClient = account.CreateCloudBlobClient();
        }

        public async Task<CloudBlob> UploadFileAsync(string container_name, string blob_name, string file_path)
        {
            using (var stream = File.OpenRead(file_path))
            {
                return await UploadStreamAsync(container_name, blob_name, stream);
            }
        }

        public async Task<CloudBlob> UploadStringAsync(string container_name, string blob_name, string string_data)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(string_data);
            MemoryStream stream = new MemoryStream(byteArray);
            return await UploadStreamAsync(container_name, blob_name, stream);
        }

        public async Task<CloudBlob> UploadStreamAsync(string container_name, string blob_name, Stream stream_data)
        {
            CloudBlockBlob blockBlob = null;
            try
            {
                // Retrieve reference to a previously created container.
                var container = blobClient.GetContainerReference(container_name);
                if (container != null)
                {
                    // Retrieve reference to a blob.
                    blockBlob = container.GetBlockBlobReference(blob_name);

                    await blockBlob.UploadFromStreamAsync(stream_data);
                }
            }
            catch (Exception x)
            {
                Debug.WriteLine(x);
                throw x;
            }
            return blockBlob;
        }
    }
}