using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TarkovAssistantWPF.data
{
    public class Bullet
    {
        public string id;
        public string name;
        public string shortName;
        public float weight;
        public string caliber;
        public int stackMaxSize;
        public bool tracer;
        public string tracerColor;
        public string ammoType;
        public int projectileCount;
        
        // since our data describes ballistics as a dictionary,
        // we'll use functions to return needed fields
        public Dictionary<string, float> ballistics;
        
        public float damage()               { return ballistics["damage"]; }
        public float armorDamage()          { return ballistics["armorDamage"]; }
        public float fragmentationChance()  { return ballistics["fragmentationChance"]; }
        public float ricochetChance()       { return ballistics["ricochetChance"]; }
        public float penetrationChance()    { return ballistics["penetrationChance"]; }
        public float penetrationPower()     { return ballistics["penetrationPower"]; }
        public float accuracy()             { return ballistics["accuracy"]; }
        public float recoil()               { return ballistics["recoil"]; }
        public float initialSpeed()         { return ballistics["initialSpeed"]; }

    }
    public class AmmoData
    {
        private static string AMMO_DATA_LOCATION = "./tarkovdata/ammunition.json";

        private static AmmoData _instance;

        private ArrayList allBullets;
        private HashSet<string> allCalibers = new HashSet<string>(); 

        private AmmoData()
        {
            allBullets = new ArrayList();
            
            var json = JObject.Parse(File.ReadAllText(AMMO_DATA_LOCATION));
            
            
            foreach (JToken child in json.Children().Children())
            {
                Bullet test = JsonConvert.DeserializeObject<Bullet>(child.ToString());
                allCalibers.Add(test.caliber);
            }
        }
        
        public static AmmoData GetInstance()
        {
            if (_instance == null)
                _instance = new AmmoData();

            return _instance;
        }

        public List<string> GetAllCalibers()
        {
            return allCalibers.ToList();
        }
    }
}