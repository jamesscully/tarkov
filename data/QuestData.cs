using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TarkovAssistantWPF.data.models;

namespace TarkovAssistantWPF.data
{
    public class QuestData : BaseDataClass<Quest>
    {
        private static QuestData _instance;

        private QuestData()
        {
            DATA_LOCATION = Constants.DATA_LOCATION_QUESTS;
            
            Load();
        }

        public override IJEnumerable<JToken> GetParseEntryPoint()
        {
            return json.Children();
        }
        
        // Not the cleanest, but allows us to use virtual method (GetParseEntryPoint) with code reuse ... terrible design!
        public override void Load(Func<Quest, bool> forEachHook = null)
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