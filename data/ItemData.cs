using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TarkovAssistantWPF.data
{
    public class Item
    {
        public string id = "undefined";
        public string name = "undefined";
        public string shortName = "undefined";

        public override string ToString()
        {
            return $"item: {name}, {shortName}; id = {id}";
        }
    }
    
    public class ItemData
    {
        private static ItemData _instance;

        public List<Item> items;

        private string ITEM_DATA_LOCATION = "./tarkovdata/items.en.json";

        private ItemData()
        {
            items = new List<Item>();
            
            var json = JObject.Parse(File.ReadAllText(ITEM_DATA_LOCATION));


            foreach (JToken child in json.Children().Children())
            {
                Item item = JsonConvert.DeserializeObject<Item>(child.ToString());
                items.Add(item);
            }
        }

        public static ItemData GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ItemData();
            }

            return _instance;
        }

        public void PrintAllItems()
        {
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
        }
    }
}