using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorKitSDK
{
    public class summary
    {

        public string deviceId { get; set; }
        public string deviceName { get; set; }
        public string tag { get; set; }
        DateTime startdt; // start session time (absolute)
        public int steps { get; set; } // number of steps since power on
        public int air { get; set; } // number of jumps since power on
        public double airgmax { get; set; } // max G force for impact at jumps m/s^2 since power on
        public double airaltmax { get; set; } // altitude max for jumps m since power on
        public double airgavg { get; set; } // avg G impact force at jumps m/s^2 since power on
        public double airt { get; set; } // time in the air while jumping sec since power on
        public long dsessions { get; set; } // number of dribble sessions since power on
        public long dribbleCount { get; set; } // total number of dribbles since power on
        public double dgmax { get; set; } // max G force of dribbles since power on
        public double dgavg { get; set; } // avg G force of dribbles since power on
        public double dpace { get; set; } // pace of dribbles in dribbles per minute since power on
    }




}
