using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorKitSDK
{

    public class SensorDiveData
    {
        public long t { get; set; } // timestamp
        public double dt { get; set; } // duration
        public double hurdles { get; set; } // hurdle count
        public double w { get; set; } // angular velocity, dps
        public double angle { get; set; } // max flexion angle, deg
        public double g { get; set; } // acceleration, m/s^2
    }

}
