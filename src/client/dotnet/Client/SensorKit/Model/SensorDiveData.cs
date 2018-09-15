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
        public long t { get; set; }
        public double dt { get; set; }
        public double count { get; set; } // hurdle count
        public double g { get; set; }
        public double w { get; set; } // angular velocity
        public double angle { get; set; } // angle
    }

}
