using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TarkovAssistantWPF.data
{

    public class Quest : BaseDataObjectClass
    {
        public int giver;
        public int turnin;
        public string title;

        public string wiki;

        public int exp;
        public string[] unlocks;

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
        
        public override string ToString()
        {
            return $"{title}, from {giver}, {wiki}";
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

        public override void Load()
        {
            Data = new Dictionary<int, Quest>();
            
            json = JToken.Parse(File.ReadAllText(DATA_LOCATION));
            
            Console.WriteLine("Laoding");
            
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