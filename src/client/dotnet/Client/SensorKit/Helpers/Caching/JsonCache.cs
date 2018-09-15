using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorKitSDK
{
    /// <summary>
    /// Used as a wrapper around the stored file to keep metadata
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheObject<T>
    {
        /// <summary>
        /// Expire date of cached file
        /// </summary>
        public DateTime? ExpireDateTime { get; set; }

        /// <summary>
        /// Actual file being stored
        /// </summary>
        public T File { get; set; }

        /// <summary>
        /// Is the cache file valid?
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (ExpireDateTime == null || ExpireDateTime.Value > DateTime.Now);
            }
        }
    }

    /// <summary>
    /// Stores objects as json in the localstorage
    /// </summary>
    public static class JsonCache
    {
        //private static readonly string CacheFolder = "_jsoncache";

        /// <summary>
        /// Get object based on key, or generate the value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="generate"></param>
        /// <param name="expireDate"></param>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        public async static Task<T> GetAsync<T>(string cacheFolder, string key, Func<Task<T>> generate, DateTime? expireDate = null, bool forceRefresh = false)
        {
            object value;

            //Force bypass of cache?
            if (!forceRefresh)
            {
                //Check cache
                value = await GetFromCache<T>(cacheFolder, key).ConfigureAwait(false);
                if (value != null)
                {
                    return (T)value;
                }
            }

            value = await generate().ConfigureAwait(false);
            await Set(cacheFolder, key, value, expireDate).ConfigureAwait(false);

            return (T)value;

        }

        /// <summary>
        /// Get value from cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async static Task<T> GetFromCache<T>(string cacheFolder, string key)
        {
            DataStorageHelper<CacheObject<T>> storage = new DataStorageHelper<CacheObject<T>>(cacheFolder);

            //Get cache value
            var value = await storage.LoadAsync(key).ConfigureAwait(false);

            if (value == null)
                return default(T);
            else if (value.IsValid)
                return value.File;
            else
            {
                //Delete old value
                //Do not await
                Delete(cacheFolder, key);

                return default(T);
            }
        }

        /// <summary>
        /// Set value in cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireDate"></param>
        /// <returns></returns>
        public static Task Set<T>(string cacheFolder, string key, T value, DateTime? expireDate = null)
        {
            DataStorageHelper<CacheObject<T>> storage = new DataStorageHelper<CacheObject<T>>(cacheFolder);

            CacheObject<T> cacheFile = new CacheObject<T>() { File = value, ExpireDateTime = expireDate };

            return storage.SaveAsync(cacheFile, key);
        }

        /// <summary>
        /// Delete key from cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Task Delete(string cacheFolder, string key)
        {
            DataStorageHelper<object> storage = new DataStorageHelper<object>(cacheFolder);
            return storage.DeleteAsync(key);
        }

        /// <summary>
        /// Clear the complete cache
        /// </summary>
        /// <returns></returns>
        public static Task ClearAll(string cacheFolder)
        {
            return Task.Run(async () =>
            {
                DataStorageHelper<object> storage = new DataStorageHelper<object>(cacheFolder);
                var folder = await storage.GetFolderAsync().ConfigureAwait(false);

                try
                {
                    await folder.DeleteAsync();
                }
                catch (UnauthorizedAccessException)
                {
                }

                //foreach (var file in await folder.GetFilesAsync())
                //{
                //    await file.DeleteAsync();
                //}

            });
        }
    }
}
