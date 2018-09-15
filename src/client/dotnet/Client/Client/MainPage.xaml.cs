// Kevin Ashley, Microsoft, 2018
// SensorKit
using Microcharts;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Xamarin.Forms;
using SensorKitSDK;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SensorKitClient
{
    public partial class MainPage : ContentPage
    {
        bool isScanned = false;
        Color mainColor = Color.DeepPink;
        Color secondaryColor = Color.DeepSkyBlue;

        #region Data Model

        // Model

        public static readonly BindableProperty ModelProperty = BindableProperty.Create(nameof(Model), typeof(DivingModel), typeof(MainPage), null, BindingMode.OneWay);

        public DivingModel Model
        {
            get { return (DivingModel)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        // Sensors

        public static readonly BindableProperty SensorsProperty = BindableProperty.Create(nameof(Sensors), typeof(ObservableCollection<SensorModel>), typeof(MainPage), null, BindingMode.OneWay);

        public ObservableCollection<SensorModel> Sensors
        {
            get { return (ObservableCollection<SensorModel>)GetValue(SensorsProperty); }
            set { SetValue(SensorsProperty, value); }
        }

        // Charts

        public static readonly BindableProperty PlotProperty = BindableProperty.Create(nameof(Plot), typeof(PlotModel), typeof(MainPage), new PlotModel() { Background = OxyColors.Transparent }, BindingMode.OneWay);

        public PlotModel Plot
        {
            get { return (PlotModel)GetValue(PlotProperty); }
            set { SetValue(PlotProperty, value); }
        }

        Chart _currentChart;
        public Chart CurrentChart
        {
            get { return _currentChart; }
            set
            {
                _currentChart = value;
                OnPropertyChanged();
            }
        }

        Chart _chartData;
        public Chart ChartData
        {
            get { return _chartData; }
            set
            {
                _chartData = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            Model = new DivingModel();
            SensorKit.Instance.Init();
            Sensors = new ObservableCollection<SensorModel>();
            BindingContext = this;
            
            mainColor = (Color)Application.Current.Resources["MainColor"];
            secondaryColor = (Color)Application.Current.Resources["SecondaryColor"];

            CreateHistoryPlot();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (!isScanned)
            {
                await SensorKit.Instance.StartScanning();
                foreach (var sensor in SensorKit.Instance.ExternalSensors)
                {
                    await sensor.Subscribe();
                    sensor.PropertyChanged += Sensor_PropertyChanged;
                    await Task.Delay(1000);
                }
                isScanned = true;
            }
           
        }

        private async void Settings_Clicked(object sender, EventArgs e)
        {
           await Navigation.PushAsync(new SettingsPage());
        }

        private async void Start(object sender, EventArgs e)
        {
            try
            {

                // reset the summary
                Model.LastSessionDt = DateTime.Now;

                if (
                        Model.isDemo
                        ||
                        Sensors?.Count > 0
                    )
                {
                    if (Model.isDemo)
                    {
                        foreach(var s in Model.DemoSensors)
                        {
                            s.DribbleOffset = 0;
                        }
                    }
                    else
                    {
                        foreach (var s in Sensors)
                        {
                            if (s.LastSummary != null)
                            {
                                s.DribbleOffset = s.LastSummary.dribbles;
                            }
                        }
                    }
                    

                    Model.isStopped = false;

                    startButton.IsVisible = false;
                    statusText.IsVisible = true;
                    chartHistory.IsVisible = false;

                    //await AnimateCountdownAsync();

                    statusText.IsVisible = false;
                    stopButton.IsVisible = true;
                    chartHistory.IsVisible = true;
                    scorePanel.IsVisible = true;
                    statsPanel.IsVisible = true;


                    (Plot.Series[0] as LineSeries).Points.Clear();
                    (Plot.Series[1] as LineSeries).Points.Clear();
                    UpdateCharts();
                    Plot.InvalidatePlot(true);

                    Device.StartTimer(TimeSpan.FromSeconds(0.8), () =>
                    {
                        try
                        {
                            if (!Model.isStopped && (Model.isDemo || Sensors.Count > 0))
                            {
                                if (Model.isDemo)
                                {
                                    Model.NextDemoData();
                                }
                                else
                                {
                                    for (int i = 0; i < Sensors.Count; i++)
                                    {
                                        var s = Sensors[i];
                                        if (s.LastSummary != null)
                                        {
                                            //var team = Settings.Current.GetTeamNumber(s);
                                            ////Debug.WriteLine($"{s.Name} {team}");
                                            //if (team != null)
                                            //{
                                            //    Device.BeginInvokeOnMainThread(() =>
                                            //    {
                                            //        if (team == 0)
                                            //        {
                                            //            Model.Team1Dribbles = s.LastSummary.dribbles - s.DribbleOffset;
                                            //        }
                                            //        else
                                            //        {
                                            //            Model.Team2Dribbles = s.LastSummary.dribbles - s.DribbleOffset;
                                            //        }
                                            //    });

                                                
                                            //}
                                        }
                                        

                                    }

                                }


                                (Plot.Series[0] as LineSeries).Points.Add(new DataPoint(Model.x, Model.Score));
                                if ((Plot.Series[0] as LineSeries).Points.Count > 30) //show only last points
                                    (Plot.Series[0] as LineSeries).Points.RemoveAt(0); //remove first point

                                Model.x++;

                                UpdateCharts();
                                Plot.InvalidatePlot(true);


                            }
                        }catch(Exception x)
                        {
                            Debug.WriteLine(x);
                        }

                        if (Model.isStopped)
                            return false;
                        else
                            return true; // True = Repeat again, False = Stop the timer
                    });


                }

            }catch(Exception x)
            {
                Debug.WriteLine(x);
            }

        }


        async Task AnimateFinishedAsync()
        {
            statusText.Text = "FINISHED";
            statusText.IsVisible = true;
        }

        private void Stop(object sender, EventArgs e)
        {
            Model.isStopped = true;
            statusText.IsVisible = false;
            startButton.IsVisible = true;
            stopButton.IsVisible = false;
            
        }

        private void Demo(object sender, EventArgs e)
        {
            Model.StartDemo();
            Start(null, null);
        }

        private void Sensor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                var sensor = sender as SensorModel;
                if (sensor != null)
                {
                    var existing = Sensors.FirstOrDefault(s => s.Name == sensor.Name);
                    if (existing != null)
                    {
                        existing = sensor;
                    }
                    else
                    {
                        Sensors.Add(sensor);
                    }

                }

                if (Sensors.Count > 0 && waitIndicator.IsRunning && Sensors[0].LastSummary != null)
                {
                    DisplayBeginSession();
                }
            }catch(Exception x)
            {
                Debug.WriteLine(x);
            }
           
            
        }

        void DisplayBeginSession()
        {
            waitIndicator.IsRunning = false;
            waitPanel.IsVisible = false;
            statusText.Text = "Ready";
            startButton.IsVisible = true;
        }

        void UpdateCharts()
        {
            List<Microcharts.Entry> data = new List<Microcharts.Entry>();
            data.Add(new Microcharts.Entry((float)Model.Score) { Color = mainColor.ToSKColor() });
            CurrentChart = new BarChart() { Entries = data };
        }

        void CreateHistoryPlot()
        {
            Plot = new PlotModel() { Background = OxyColors.Transparent, LegendPlacement = LegendPlacement.Outside, LegendOrientation = LegendOrientation.Horizontal, LegendPosition = LegendPosition.BottomCenter };
            Plot.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "COUNT", Unit = "count", MaximumPadding = 0.1 });
            Plot.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, MaximumPadding = 0.1, IsAxisVisible = false });

            Plot.Series.Add(new LineSeries()
            {
                Color = mainColor.ToOxyColor(),
                StrokeThickness = 6,
                MarkerSize = 2
            });

            chartHistory.Model = Plot;
        }

        
    }

   
}
