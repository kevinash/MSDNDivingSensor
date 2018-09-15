using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using HockeyApp.Android;

namespace SensorKitClient.Droid
{
    [Activity(Label = "SensorKitClient", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();
            LoadApplication(new App());
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
        }

        protected override void OnResume()
        {
            base.OnResume();
            CrashManager.Register(this, "8271bb6c70fe48b4b04f4ad2a529a582");
        }

    }
}

