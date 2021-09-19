using System;
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
    
    public class TraderData : BaseDataClass<Trader>
    {
        private TraderData _instance;
        
        

        private TraderData()
        {
            DATA_LOCATION = "./tarkovdata/traders.json";
            Load();
        }

        public TraderData GetInstance()
        {
            if (_instance == null)
            {
                _instance = new TraderData();
            }

            return _instance;
        }
        
    }
}