using SensorKitSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace SensorKitClient
{
    public class DivingModel : BaseViewModel
    {
        // All measurements in SensorKit are in International System of Units (SI) - metric
        // conversion to imperial provided for visualization only
        
        DateTime lastSessionDt = DateTime.MinValue;
        public DateTime LastSessionDt { get => lastSessionDt; set => SetProperty(ref lastSessionDt, value); }

        public bool isStopped { get; set; } = false;
        public bool isDemo { get; set; } = false;

        public double x { get; set; }
        
        
        // demo mode

        public List<SensorModel> DemoSensors { get; set; } = new List<SensorModel>();

        public void StartDemo()
        {
            isDemo = true;
            DemoSensors = new List<SensorModel>();
            // add 4 sensors
            for (int i = 0; i < 4; i++)
            {
                DemoSensors.Add(new SensorModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"SensorKit S1 0000000{i}",
                    Tag = i.ToString()
                });
            }
        }

        public void NextDemoData()
        {
            var rnd = new Random();
            int d1 = rnd.Next(0, 6);
            Score += d1;
            

            try
            {
                Task.Run(() => SensorModel.PostToApi(DemoSensors[0], new SensorItem { itemType = SensorItemTypes.Dribble, dribbles = d1, duration = GetRandomNumber(0.01, 0.3), dgavg = GetRandomNumber(10.0, 16.0), dgmax = GetRandomNumber(12.0, 20.0), pace = GetRandomNumber(3.0, 10.0) }));
            }catch(Exception x)
            {
                Debug.WriteLine(x);
            }
        }

        public static double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        int score = 0;
        public int Score { get => score; set => SetProperty(ref score, value); }


    }
}
