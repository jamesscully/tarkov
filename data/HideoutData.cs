using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TarkovAssistantWPF.data
{

    public class HideoutStation
    {
        public int id = -1;

        public Dictionary<string, string> locales;

        public string function = "";
        public string imgSource = "";

        // used for events - christmas tree station 
        public bool disabled;

        public string name
        {
            get
            {
                return locales["en"];
            }
        }
    }
    
    public class Module
    {

        public Requirement[] require;
        
        public string module = "";
        public int level = -1;

        public int id;
        public int stationId;
        
        public class Requirement
        {
            public int id;

            public string type;
            public string name;
            
            // can also serve as a required modules 'level'
            public int quantity;
        }
    }
    
    

    
    public class HideoutData : BaseDataClass<HideoutStation>
    {
        private static HideoutData _instance;

        private HideoutData()
        {

            DATA_LOCATION = Constants.DATA_LOCATION_HIDEOUT;
            
            Load();
        }

        public static HideoutData GetInstance()
        {
            if (_instance == null)
            {
                _instance = new HideoutData();
            }

            return _instance;
        }

        public override IJEnumerable<JToken> GetParseEntryPoint()
        {
            return json.Children();
        }

        public override void Load(Func<HideoutStation, bool> forEachHook = null)
        {
            
            json = JObject.Parse(File.ReadAllText(DATA_LOCATION));
            
            foreach (JToken child in GetParseEntryPoint())
            {
                if (child is JProperty)
                {
                    var prop = child as JProperty;

                    // both stations and module keys
                    // contain arrays as of 27/9
                    var array = prop.Value as JArray;

                    if (prop.Name == "stations")
                    {
                        ParseAllStations(array);
                    }
                    else if (prop.Name == "modules")
                    {
                        ParseAllModules(array);
                    }
                }
            }
        }

        public void ParseAllStations(JArray stationRoot)
        {
            foreach (var token in stationRoot)
            {
                HideoutStation station = JsonConvert.DeserializeObject<HideoutStation>(token.ToString(), new JsonSerializerSettings()
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                });
                
                Console.WriteLine($"Found {station.name}, id: {station.id}");
            }
        }

        public void ParseAllModules(JArray moduleRoot)
        {
            foreach (var token in moduleRoot)
            {
                Module module = JsonConvert.DeserializeObject<Module>(token.ToString(), new JsonSerializerSettings()
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                });
                
                Console.WriteLine($"Found module {module.module}, id: {module.id}");
            }
        }
    }
}