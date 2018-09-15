using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorKitSDK
{
    public static class ModelExtensions
    {
        public static string ToJsonString(this dribblesession dribbleSession)
        {
            var sb = new StringBuilder();
            sb.Append("{");

            if (dribbleSession.deviceId != "")
            {
                sb.Append("\"deviceId\":");
                sb.Append("\"");
                sb.Append(dribbleSession.deviceId);
                sb.Append("\"");
            }

            if (dribbleSession.deviceName != "")
            {
                sb.Append(", ");
                sb.Append("\"deviceName\":");
                sb.Append("\"");
                sb.Append(dribbleSession.deviceName);
                sb.Append("\"");
            }

            if (dribbleSession.tag != "")
            {
                sb.Append(", ");
                sb.Append("\"tag\":");
                sb.Append("\"");
                sb.Append(dribbleSession.tag);
                sb.Append("\"");
            }

            if (dribbleSession.startdt != null)
            {
                sb.Append(", ");
                sb.Append("\"startdt\":");
                sb.Append("\"");
                sb.Append(dribbleSession.startdt);
                sb.Append("\"");
            }

            if (dribbleSession.duration != null)
            {
                sb.Append(", ");
                sb.Append("\"duration\":");
                sb.Append(dribbleSession.duration);
            }

            if (dribbleSession.count != null)
            {
                sb.Append(", ");
                sb.Append("\"count\":");
                sb.Append(dribbleSession.count);
            }

            if (dribbleSession.gavg != null)
            {
                sb.Append(", ");
                sb.Append("\"gavg\":");
                sb.Append(dribbleSession.gavg);
            }

            if (dribbleSession.gmax != null)
            {
                sb.Append(", ");
                sb.Append("\"gmax\":");
                sb.Append(dribbleSession.gmax);
            }

            if (dribbleSession.pace != null)
            {
                sb.Append(", ");
                sb.Append("\"pace\":");
                sb.Append(dribbleSession.pace);
            }

            if (dribbleSession.heatId != null)
            {
                sb.Append(", ");
                sb.Append("\"heatId\":");
                sb.Append(dribbleSession.heatId);
            }

            if (dribbleSession.drill != null && dribbleSession.drill.Trim() != "")
            {
                sb.Append(", ");
                sb.Append("\"drill\":");
                sb.Append("\"");
                sb.Append(dribbleSession.drill);
            }

            sb.Append("}");

            return sb.ToString();

            //return "{\"deviceId\":\"00000000 - 0000 - 0000 - 0000 - c974a2c13fa7\",\"deviceName\":\"SensorKit S1 964C478B\",\"tag\":null,\"duration\":0.0,\"count\":1,\"gavg\":0.0,\"gmax\":0.0,\"pace\":0.0}";
        }

    }
}
