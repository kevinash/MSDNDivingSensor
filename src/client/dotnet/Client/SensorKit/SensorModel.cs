// Kevin Ashley, Microsoft, 2018
// SensorKit
using Newtonsoft.Json;
using PCLStorage;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace SensorKitSDK
{
    public delegate void SensorModelStopHandler();
    public delegate void ValueChangedHandler(SensorItem newValue);
    
    [DataContract]
    public class SensorModel :  ViewModel, ISensorModel
    {
        public event SensorModelStopHandler Stopped;
        public event ValueChangedHandler ValueChanged;
    
        [DataMember]
        public Guid Id { get; set; }

        
        string _name;
        [DataMember]
        public string Name {
            get
            {
                return _name;
            }
            set
            {
                SetValue(ref _name, value, "Name");
            }
        }

        string _tag;

        [DataMember]
        public string Tag
        {

            get
            {
                return _tag;
            }

            set
            {
                SetValue(ref _tag, value, "Tag");
            }
        }

        bool _isLive;
        [IgnoreDataMember]
        public bool IsLive
        {
            get
            {
                return _isLive;
            }
            set
            {
                SetValue(ref _isLive, value, "IsLive");
            }
        }

        [IgnoreDataMember]
        public IConnector Connector { get; set; }

        SensorInformation _info;
        [DataMember]
        public SensorInformation Information { get { return _info; } set { SetValue(ref _info, value, "Information"); } }

        [IgnoreDataMember]
        public List<SensorItem> History { get; set; } = new List<SensorItem>();

        [IgnoreDataMember]
        public DateTime? LastSync
        {
            get
            {
                if (History != null)
                {
                    var last = History.LastOrDefault(s=>s.itemType == SensorItemTypes.Summary);
                    if (last != null)
                        return last.timestamp;
                }
                return null;
            }
        }

        [IgnoreDataMember]
        public SensorItem LastSummary
        {
            get
            {
                if (History != null)
                {
                    return History.LastOrDefault(h=>h.itemType == SensorItemTypes.Summary);
                }
                return null;
            }
        }

        [IgnoreDataMember]
        public SensorItem LastDribble
        {
            get
            {
                if (History != null)
                {
                    return History.LastOrDefault(h => h.itemType == SensorItemTypes.Dribble);
                }
                return null;
            }
        }

        [IgnoreDataMember]
        public IEnumerable<SensorItem> DribbleHistory
        {
            get
            {
                if (History != null)
                {
                    return from h in History where h.itemType == SensorItemTypes.Dribble select h;
                }
                return null;
            }
        }

        [IgnoreDataMember]
        public IEnumerable<SensorItem> AirHistory
        {
            get {
                if(History != null)
                {
                    return from h in History where h.itemType == SensorItemTypes.Airtime select h;
                }
                return null;
            }
        }

        [IgnoreDataMember]
        public IEnumerable<SensorItem> TurnHistory
        {
            get
            {
                if (History != null)
                {
                    return from h in History where h.itemType == SensorItemTypes.Turns select h;
                }
                return null;
            }
        }


        [IgnoreDataMember]
        public IEnumerable<SensorItem> SummaryHistory
        {
            get
            {
                if (History != null)
                {
                    return from h in History where h.itemType == SensorItemTypes.Summary select h;
                }
                return null;
            }
        }


        [IgnoreDataMember]
        public int Count {
            get {
                if (History != null)
                {
                    return History.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        [IgnoreDataMember]
        public double DribbleOffset { get; set; }

        SensorItem _value = new SensorItem();
        [IgnoreDataMember]
        public SensorItem Value { get { return _value; } set { SetValue(ref _value, value, "Value"); } }

        SensorSummaryData _summary = new SensorSummaryData();
        [IgnoreDataMember]
        public SensorSummaryData Summary { get { return _summary; } set { SetValue(ref _summary, value, "Summary"); } }

        [IgnoreDataMember]
        public bool IsSubscribed
        {
            get
            {
                if (Id == default(Guid))
                    return true;
                if (Connector != null)
                    return Connector.IsConnected;
                else
                    return false;
            }
        }

        public async Task SetLogging(bool isLogging)
        {
            if (Connector != null)
            {
                await Connector.SetLogging(isLogging);
            }
                
        }

        public async Task SetAutoUpdates(bool isAutoUpdates)
        {
            if (Connector != null)
            {
                await Connector.SetAutoUpdates(isAutoUpdates);
            }

        }

       
        public SensorModel()
        {
        }

        bool isAppending = false;
        bool isStarting = false;
        bool isStarted = false;

        public void Start()
        {
            try
            {
                if (!isStarting && !isStarted)
                {
                    isStarting = true;
                    isStarted = true;
                }
                isStarting = false;
            }
            catch { }
        }

        public void SynchronizeTime(DateTime appConnectionTime, double connectedDeviceMs)
        {
            try
            {
                var count = History.Count;
                for (int i = 0; i < count; i++)
                {
                    var item = History[i];
                    if (item != null && item.timestamp == DateTime.MinValue) // not sync'd yet and more than the connection time
                    {
                        item.timestamp = appConnectionTime.AddMilliseconds(item.offsetMs - connectedDeviceMs);
                        Debug.WriteLine($"SYNC TIME {item.timestamp} {item.offsetMs}");
                    }
                }

            }
            catch(Exception x)
            {
                Debug.WriteLine(x);
            }
        }


        public void Append(SensorItem e)
        {
            try
            {
                InvokeHelper.Invoke(() =>
                {
                    Value = e;
                    History.Add(e);
                    ValueChanged?.Invoke(e);
                    NotifyPropertyChanged("History");
                    NotifyPropertyChanged("LastSummary");
                    NotifyPropertyChanged("LastDribble");
                    NotifyPropertyChanged("DribbleHistory");
                });

                Task.Run(() => PostToApi(this, e));

         
            }
            catch(Exception x) {
                Debug.WriteLine(x);
            }
        }

        public static async Task<bool> PostToApi(SensorModel sensor, SensorItem item)
        {
            try
            {
                if (String.IsNullOrEmpty(SensorKit.Instance.API))
                    return false;

                string rootUrl = SensorKit.Instance.API;
                string url = rootUrl;
                var client = new HttpClient();

                HttpResponseMessage response;

                string json = "";

                if (item.itemType == SensorItemTypes.Dribble)
                {
                    url = rootUrl + "dribblesession";
                    var dribblesession = new dribblesession
                    {
                        deviceId = sensor.Id.ToString().ToLower(),
                        deviceName = sensor.Name,
                        count = item.dribbles,
                        duration = item.duration,
                        gavg = item.dgavg,
                        gmax = item.dgmax,
                        pace = item.pace,
                        tag = sensor.Tag
                    };
                    json = dribblesession.ToJsonString();
                }
                //else if (item.itemType == SensorItemTypes.Summary)
                //{
                //    url = rootUrl + "dribblesummary";
                //    var dribblesummary = new summary
                //    {
                //        deviceId = Id.ToString().ToLower(),
                //        deviceName = Name,
                //        tag = Tag,
                //        air = item.aircount,
                //        airaltmax = item.airaltmax,
                //        airgavg = item.airgavg,
                //        airgmax = item.airgmax,
                //        airt = item.airt,
                //        steps = (int)item.steps,
                //        dribbleCount = item.dribbles,
                //        dgavg = item.dgavg,
                //        dgmax = item.dgmax,
                //        dpace = item.pace,
                //        dsessions = item.sessions
                //    };
                //    json = dribblesummary.ToJsonString();
                //}

                if (!String.IsNullOrEmpty(json))
                {
                    byte[] byteData = Encoding.UTF8.GetBytes(json);

                    using (var content = new ByteArrayContent(byteData))
                    {
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        response = await client.PostAsync(url, content);
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                }

            }catch(Exception x)
            {
                Debug.WriteLine(x);
            }

            return false;
            
        }

        

        public void Save()
        {
            InvokeHelper.Invoke(() =>
            {
                ValueChanged?.Invoke(null);
            });

           
        }

        public async Task Forget()
        {
            Stop(); // stop data
                           // unsubscribe from the bluetooth
            if (Connector != null)
            {
                await Connector.Unsubscribe();
                Connector = null;
            }
            // untag the sensor
            Tag = null;
            NotifyPropertyChanged("IsConnected");
        }

        public async Task Subscribe()
        {
            if (Connector == null)
            {
                Connector = new SensorKitConnector(this);
            }
            await Connector.Subscribe();
        }

        public async Task Unsubscribe()
        {
            await Connector?.Unsubscribe();
        }

        public void Stop()
        {
            try
            {
                Save();
                isStarted = false;
                Stopped?.Invoke();
            }
            catch(Exception x) {
                Debug.WriteLine(x);
            }
        }


        
    }
}




