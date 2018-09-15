// Kevin Ashley, Microsoft, 2018
// SensorKit
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using SensorKitSDK;

namespace SensorKitClient
{
    public class Settings : BaseViewModel
    {
        static ISettings AppSettings =>
          CrossSettings.Current;

        static Settings settings;
        public static Settings Current =>
          settings ?? (settings = new Settings());

        public List<string> SensorFilterShortNames { get; set; } = new List<string>();

        public Settings()
        {
            if (!String.IsNullOrEmpty(SensorFilter))
            {
                var names = SensorFilter.Split(',');
                foreach (var n in names)
                {
                    if (!SensorFilterShortNames.Contains(n))
                    {
                        SensorFilterShortNames.Add(n);
                    }
                }
            }
        }

        public string SensorFilter
        {
            get => AppSettings.GetValueOrDefault(nameof(SensorFilter), "364A");
            set
            {
                var original = SensorFilter;
                if (AppSettings.AddOrUpdateValue(nameof(SensorFilter), value))
                    SetProperty(ref original, value);
            }
        }

        public string ClientAPI
        {
            get => AppSettings.GetValueOrDefault(nameof(ClientAPI), "http://PLEASE-PROVIDE-API-URL/");
            set
            {
                var original = ClientAPI;
                if (AppSettings.AddOrUpdateValue(nameof(ClientAPI), value))
                    SetProperty(ref original, value);
                if(!String.IsNullOrEmpty(value))
                    SensorKit.Instance.API = value;

            }
        }

        public string Drill
        {
            get => AppSettings.GetValueOrDefault(nameof(Drill), "dribble");
            set
            {
                var original = Drill;
                if (AppSettings.AddOrUpdateValue(nameof(Drill), value))
                    SetProperty(ref original, value);

            }
        }


    }
}
