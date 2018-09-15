using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorKitSDK
{
    
    public class dribblesession
    {
        public string deviceId { get; set; }
        public string deviceName { get; set; } // device name
        public string tag { get; set; } //device tag, can be anything (e.g. LeftLowerHand)
        public DateTime startdt { get; set; } // start session time (absolute)
        public double duration { get; set; } // duration in sec
        public long count { get; set; } // dribble count
        public double gavg { get; set; } // avg G force for this session m/s^2
        public double gmax { get; set; } // max G force for this session m/s^2
        public double pace { get; set; } // pace of dribbles in dribbles/minute (in this session)
        public long heatId { get; set; } // heat is a sequence of sessions
        public string drill { get; set; }
    }

    


}
