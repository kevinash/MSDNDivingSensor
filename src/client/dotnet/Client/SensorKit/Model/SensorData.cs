using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorKitSDK
{
    public class SensorData
    {
        public SensorAirData airdata { get; set; }
        public SensorTurnData turndata { get; set; }
        public SensorDribbleData dribbledata { get; set; }
        public SensorRawData rawdata { get; set; }
        public SensorHurdleData hurdledata { get; set; }
        public SensorDiveData divedata { get; set; }
        public SensorSummaryData summary { get; set; }
    }
}
