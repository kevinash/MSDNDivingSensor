// Kevin Ashley, Microsoft, 2018
// SensorKit
using SensorKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SensorKitClient
{
	public partial class SettingsPage : ContentPage
	{
		public SettingsPage ()
		{
			InitializeComponent ();
            BindingContext = Settings.Current;
		}

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            //var detected = from s in SensorKit.Instance.ExternalSensors select s.Name;
            //detectedSensors.Text = String.Join(",", detected);
           
        }

       
        
    }
}