using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TarkovAssistantWPF.data
{

    // Object representations of Json for automatic deserialization
    public class Quest : BaseDataObjectClass
    {
        public int giver;
        public int turnin;
        public string title;

        public string wiki;

        public int exp;
        public string[] unlocks;

        public Requirements require;
        public Reputation[] reputation;
        public Objective[] objectives;

        public string gameId;
        
        public class Objective
        {
            public string type;
            public string tool;
            public object target;
            public string hint;
            public string[] with;
            public int number;
            public int location;
            public int id;
        }

        public class Reputation
        {
            public int trader;
            public float rep;
        }

        public class Requirements
        {
            public int level;
            public int[] quests;
        }
        
        public override string ToString()
        {
            float x = -1;
            if (reputation.Length > 0)
            {
                x = reputation[0].rep;
            }
            return $"{title}, from {giver}, {wiki} {x}";
        }

    }
    
    public class QuestData : BaseDataClass<Quest>
    {
        private static QuestData _instance;

        private QuestData()
        {
            DATA_LOCATION = "./tarkovdata/quests.json";
            
            Load();
        }

        public override IJEnumerable<JToken> GetParseEntryPoint()
        {
            return json.Children();
        }
        
        // Not the cleanest, but allows us to use virtual method... terrible design!
        public override void Load()
        {
            Data = new Dictionary<int, Quest>();
            
            json = JToken.Parse(File.ReadAllText(DATA_LOCATION));

            foreach (JToken child in GetParseEntryPoint())
            {
                Quest data = JsonConvert.DeserializeObject<Quest>(child.ToString());
                Console.WriteLine("Loading data: " + data);
                Data.Add(data.GetHashCode(), data);
            }
        }

        public static QuestData GetInstance()
        {
            if (_instance == null)
            {
                _instance = new QuestData();
            }

            return _instance;
        }
    }
}