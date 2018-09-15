// Kevin Ashley, Microsoft, 2018
// SensorKit
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorKitSDK
{
    public class SensorsRegistry
    {
        List<SensorInformation> _registry = new List<SensorInformation>()
        {
            new SensorInformation{ Model=SensorTypes.Hub, ShortDescription = "Sensor Kit", Capabilities = new List<SensorCapabilities>{SensorCapabilities.Hub } },
            new SensorInformation{ Model=SensorTypes.S1, Manufacturer="SensorKit", Url = "https://sensorkit.org/products/s1dk", ShortDescription = "SensorKit S1",  Capabilities = new List<SensorCapabilities> {SensorCapabilities.IMU, SensorCapabilities.AirTime, SensorCapabilities.Battery, SensorCapabilities.Ski, SensorCapabilities.TelemarkSki, SensorCapabilities.Snowboard, SensorCapabilities.Kiteboarding, SensorCapabilities.Surfing, SensorCapabilities.MountainBike, SensorCapabilities.Skateboard, SensorCapabilities.Windsurfing}, Format="S1"},
           // add your sensors...
        };

        public IEnumerable<SensorInformation> Public
        {
            get
            {
                return _registry.Where(s=>s.Model != SensorTypes.Hub);
            }
        }

        public IEnumerable<SensorInformation> Items
        {
            get
            {
                return _registry;
            }
        }

        public SensorsRegistry() { }

    }

    
}
