using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorKitSDK
{

    public class SensorHurdleData
    {
        public long t { get; set; } // timestamp
        public double dt { get; set; } // duration
        public double angle { get; set; } // flexion angle, deg

        public double w { get; set; } // angular velocity, dps

        public double g { get; set; } // acceleration
        
    }

}
