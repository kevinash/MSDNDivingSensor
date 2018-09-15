using PCLStorage;
using PCLStorage.Exceptions;
using SensorKitSDK.Client.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SensorKitSDK
{
    public static class SensorKitExtensions
    {
        
        public static Task<T> BeginInvokeOnMainThreadAsync<T>(Func<T> a)
        {
            var tcs = new TaskCompletionSource<T>();
            InvokeHelper.Invoke(() =>
            {
                try
                {
                    var result = a();
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }

        public static string GetHashName(string name)
        {
            if (name != null)
                return $"SensorKit{Convert.ToBase64String(Encoding.UTF8.GetBytes(name)).Replace("=", "")}";
            else
                return null;
        }

        public static byte[] StringToByteArray(this String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string ToHexString(this byte[] bytes)
        {
            return bytes != null ? BitConverter.ToString(bytes) : string.Empty;
        }

        public static Device RegisterDeviceFromSensorModel(this SessionDataContainer sessionContainer, SensorModel sensorModel)
        {
            var match = (from d in sessionContainer.Session.Devices
                         where d.Id == sensorModel.Id.ToString()
                         select d).FirstOrDefault();
            if (match == null)
            {
                var device = new Device()
                {
                    Id = sensorModel.Id.ToString(),
                    SessionId = sessionContainer.Session.Id
                };
                sessionContainer.Session.Devices.Add(device);

                return device;
            }
            else
                return match;
        }

        /// <summary>
        /// Extension method to check if file exist in folder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<bool> ContainsFileAsync(this IFolder folder, string fileName)
        {
            //This looks nicer, but gave a COM errors in some situations
            //TODO: Check again in final release of Windows 8 (or 9, or 10)
            //return (await folder.GetFilesAsync()).Where(file => file.Name == fileName).Any();

            try
            {
                var result = await folder.CheckExistsAsync(fileName);
                return result == ExistenceCheckResult.FileExists;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }

    }
}
