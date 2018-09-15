using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PCLStorage;

namespace SensorKitSDK
{
    /// <summary>
    /// Save object to local storage, serializes as json and writes object to a file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataStorageHelper<T>
    {
        private readonly string _fileExtension = ".json";

        private IFileSystem _appData = FileSystem.Current;
        private string _subFolder;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="StorageType"></param>
        /// <param name="subFolder"></param>
        public DataStorageHelper(string subFolder = null)
        {
            _subFolder = subFolder;
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="fileName"></param>
        public async Task DeleteAsync(string fileName)
        {
            fileName = fileName + _fileExtension;
            try
            {
                IFolder folder = await GetFolderAsync().ConfigureAwait(false);

                var contains = await folder.ContainsFileAsync(fileName).ConfigureAwait(false);
                if (contains)
                {
                    var file = await folder.GetFileAsync(fileName);
                    await file.DeleteAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Save object from file
        /// </summary>
        /// <param name="Obj"></param>
        /// <param name="fileName"></param>
        public async Task SaveAsync(T Obj, string fileName)
        {

            fileName = fileName + _fileExtension;
            try
            {
                if (Obj != null)
                {
                    //Get file
                    IFile file = null;
                    IFolder folder = await GetFolderAsync().ConfigureAwait(false);
                    file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);


                    //Serialize object
                    
                    var json = JsonConvert.SerializeObject(Obj);

                    // Write the data from the textbox.
                    byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(json.ToCharArray());

                    using (var s = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                    {
                        s.Write(fileBytes, 0, fileBytes.Length);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Load object from file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<T> LoadAsync(string fileName)
        {
            fileName = fileName + _fileExtension;
            try
            {

                IFile file = null;
                IFolder folder = await GetFolderAsync().ConfigureAwait(false);

                var contains = await folder.ContainsFileAsync(fileName).ConfigureAwait(false);
                if (contains)
                {
                    file = await folder.GetFileAsync(fileName);


                    try
                    {

                        var fileStream = await file.OpenAsync(PCLStorage.FileAccess.Read);
                        // Read the data.
                        using (StreamReader streamReader = new StreamReader(fileStream))
                        {
                            var data = streamReader.ReadToEnd();
                            //Deserialize to object
                            T result = JsonConvert.DeserializeObject<T>(data);
                            return result;
                        }
                    }
                    catch
                    {
                        return default(T);
                    }

                    
                }
                else
                {
                    return default(T);
                }

            }
            catch (Exception x)
            {
                //Unable to load contents of file
                //Logger.WriteLine(x);
                throw;
            }
        }

        /// <summary>
        /// Get folder based on storagetype
        /// </summary>
        /// <returns></returns>
        public async Task<IFolder> GetFolderAsync()
        {
            IFolder folder =  _appData.LocalStorage;
            if (!string.IsNullOrEmpty(_subFolder))
            {
                folder = await folder.CreateFolderAsync(_subFolder, CreationCollisionOption.OpenIfExists);
            }
            return folder;
        }

        /// <summary>
        /// Clear the complete cache
        /// </summary>
        /// <returns></returns>
        public static Task ClearLocalAll()
        {
            return Task.Run(async () =>
            {
                DataStorageHelper<object> storage = new DataStorageHelper<object>();
                var folder = await storage.GetFolderAsync().ConfigureAwait(false);

                foreach (var sub in await folder.GetFoldersAsync())
                {
                    try
                    {
                        await sub.DeleteAsync();
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }

                foreach (var file in await folder.GetFilesAsync())
                {
                    try
                    {
                        await file.DeleteAsync();
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }
            });
        }

    }
}
