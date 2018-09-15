using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorKitSDK
{
    
    public class SensorDribbleData
    {
        public long t { get; set; }
        public double dt { get; set; }
        public long c { get; set; }
        public double gavg { get; set; }
        public double gmax { get; set; }
        public double p { get; set; }
    }

    


}
