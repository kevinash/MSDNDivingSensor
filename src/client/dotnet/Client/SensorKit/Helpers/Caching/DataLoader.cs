using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SensorKitSDK
{
    /// <summary>
    /// Possible loading states for the DataLoader
    /// </summary>
    public enum LoadingState
    {
        /// <summary>None</summary>
        None,
        /// <summary>Loading</summary>
        Loading,
        /// <summary>Finished</summary>
        Finished,
        /// <summary>Error</summary>
        Error
    }

    /// <summary>
    /// DataLoader that enables easy binding to Loading / Finished / Error properties
    /// </summary>
    public class DataLoader : INotifyPropertyChanged
    {
        private LoadingState _loadingState;
        private bool _catchExceptions = false;

        /// <summary>
        /// Current loading state
        /// </summary>
        public LoadingState LoadingState
        {
            get { return _loadingState; }
            set
            {
                _loadingState = value;

                RaisePropertyChanged();
                RaisePropertyChanged(() => IsBusy);
                RaisePropertyChanged(() => IsError);
                RaisePropertyChanged(() => IsFinished);
            }
        }

        /// <summary>
        /// Indicates LoadingState == LoadingState.Error
        /// </summary>
        public bool IsError
        {
            get
            {

                if (LoadingState == LoadingState.Error)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Indicates LoadingState == LoadingState.Loading
        /// </summary>
        public bool IsBusy
        {
            get
            {
                if (LoadingState == LoadingState.Loading)
                    return true;

                return false;
            }

        }

        /// <summary>
        /// Indicates LoadingState == LoadingState.Finished
        /// </summary>
        public bool IsFinished
        {
            get
            {
                if (LoadingState == LoadingState.Finished)
                    return true;

                return false;
            }

        }


        /// <summary>
        /// DataLoader constructors
        /// </summary>
        /// <param name="catchExceptions">Swallows exceptions</param>
        public DataLoader(bool? catchExceptions = null)
        {
            if (catchExceptions.HasValue)
                _catchExceptions = catchExceptions.Value;
        }




        /// <summary>
        ///  Load data. Errors will be in errorcallback
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loadingMethod"></param>
        /// <param name="resultCallback"></param>
        /// <param name="errorCallback">optional error callback. Fires when exceptino is thrown in loadingMethod</param>
        /// <returns></returns>
        public async Task<T> LoadAsync<T>(Func<Task<T>> loadingMethod, Action<T> resultCallback = null, Action<Exception> errorCallback = null)
        {
            //Set loading state
            LoadingState = LoadingState.Loading;

            T result = default(T);

            try
            {
                result = await loadingMethod();

                //Set finished state
                LoadingState = LoadingState.Finished;

                if (resultCallback != null)
                    resultCallback(result);

            }
            catch (Exception e)
            {
                //Set error state
                LoadingState = LoadingState.Error;

                if (errorCallback != null)
                    errorCallback(e);
                else if (!_catchExceptions) //swallow exception if catchexception is true
                    throw; //throw error if no callback is defined

            }

            return result;
        }


        /// <summary>
        /// First returns result callback with result from cache, then from refresh method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheLoadingMethod"></param>
        /// <param name="refreshLoadingMethod"></param>
        /// <param name="resultCallback"></param>
        /// <param name="errorCallback"></param>
        /// <returns></returns>
        public async Task LoadCacheThenRefreshAsync<T>(Func<Task<T>> cacheLoadingMethod, Func<Task<T>> refreshLoadingMethod, Action<T> resultCallback = null, Action<Exception> errorCallback = null)
        {
            //Set loading state
            LoadingState = LoadingState.Loading;

            T cacheResult = default(T);
            T refreshResult = default(T);

            try
            {
                cacheResult = await cacheLoadingMethod();

                if (resultCallback != null)
                    resultCallback(cacheResult);

                refreshResult = await refreshLoadingMethod();

                if (resultCallback != null)
                    resultCallback(refreshResult);

                //Set finished state
                LoadingState = LoadingState.Finished;

            }
            catch (Exception e)
            {
                //Set error state
                LoadingState = LoadingState.Error;

                if (errorCallback != null)
                    errorCallback(e);
                else if (!_catchExceptions) //swallow exception if catchexception is true
                    throw; //throw error if no callback is defined

            }

        }

        /// <summary>
        /// Loads data from source A, if this fails, load it from source B (cache)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="refreshLoadingMethod"></param>
        /// <param name="cacheLoadingMethod"></param>
        /// <param name="resultCallback"></param>
        /// <param name="errorCallback"></param>
        /// <returns></returns>
        public async Task LoadFallbackToCacheAsync<T>(Func<Task<T>> refreshLoadingMethod, Func<Task<T>> cacheLoadingMethod, Action<T> resultCallback = null, Action<Exception> errorCallback = null)
        {
            //Set loading state
            LoadingState = LoadingState.Loading;

            T refreshResult = default(T);
            T cacheResult = default(T);

            bool refreshSourceFail = false;

            try
            {
                refreshResult = await refreshLoadingMethod();
                if (resultCallback != null)
                    resultCallback(refreshResult);

                //Set finished state
                LoadingState = LoadingState.Finished;
            }
            catch (Exception e)
            {
                refreshSourceFail = true;

                if (errorCallback != null)
                    errorCallback(e);
            }

            //Did the loading fail? Load data from source B (cache)
            if (refreshSourceFail)
            {
                try
                {
                    cacheResult = await cacheLoadingMethod();
                    if (resultCallback != null)
                        resultCallback(cacheResult);

                    //Set finished state
                    LoadingState = LoadingState.Finished;
                }
                catch (Exception e)
                {
                    //Set error state
                    LoadingState = LoadingState.Error;

                    if (errorCallback != null)
                        errorCallback(e);
                }
            }
        }




        /// <summary>
        /// PropertyChanged for INotifyPropertyChanged implementation
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// RaisePropertyChanged for INotifyPropertyChanged implementation
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// RaisePropertyChanged for INotifyPropertyChanged implementation
        /// </summary>
        /// <param name="expression"></param>
        protected void RaisePropertyChanged(Expression<Func<object>> expression)
        {
            RaisePropertyChanged(GetPropertyName(expression));
        }

        /// <summary>
        /// Gets a property name, usage: GetPropertyName(() => Object.PropertyName)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;


            if (memberExpression == null)
                memberExpression = (MemberExpression)((UnaryExpression)expression.Body).Operand;


            return memberExpression.Member.Name;
        }

    }
}

