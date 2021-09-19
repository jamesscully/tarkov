using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TarkovAssistantWPF.data
{
    
    public class Trader
    {
        public enum NameID
        {
            PRAPOR = 0,
            THERAPIST,
            SKIER,
            PEACEKEEPER,
            MECHANIC,
            RAGMAN,
            JAEGER,
            FENCE
        }
        public class TraderLoyalty
        {
            public int level = -1;
            public int requiredLevel = -1;
            public float requiredReputation = -1.0f;
            public int requiredSales = -1;
        }
        
        public int id = -1;
        public string name = "undefined";
        public string locale = "undefined";
        public string wiki_url = "undefined";

        public string description = "undefined";

        public string[] currencies;

        public TraderLoyalty[] loyalty;


        public override string ToString()
        {
            return $"trader: {name}";
        }
    }
    
    public class TraderData
    {
        private TraderData _instance;

        public Dictionary<int, Trader> traders;

        public string DATA_LOCATION = "./tarkovdata/traders.json";
        
        private TraderData()
        {
            traders = new Dictionary<int, Trader>();
            
            var json = JObject.Parse(File.ReadAllText(DATA_LOCATION));


            foreach (JToken child in json.Children().Children())
            {
                Trader trader = JsonConvert.DeserializeObject<Trader>(child.ToString());
                traders.Add(trader.id, trader);
            }
        }

        public TraderData GetInstance()
        {
            if (_instance == null)
            {
                _instance = new TraderData();
            }

            return _instance;
        }

        public Trader GetTraderById(int id)
        {
            return traders[id];
        }
    }
}