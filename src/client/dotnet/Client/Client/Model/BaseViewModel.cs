// Kevin Ashley, Microsoft, 2018
// SensorKit
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SensorKitClient
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public Settings Settings => Settings.Current;

        protected bool SetProperty<T>(
            ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string name = "") =>
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        #endregion
    }
}
