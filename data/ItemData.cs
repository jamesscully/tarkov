using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TarkovAssistantWPF.interfaces;

namespace TarkovAssistantWPF.data
{
    public class Item : BaseDataObjectClass
    {
        public string name = "undefined";
        public string shortName = "undefined";

        public override string ToString()
        {
            return $"item: {name}, {shortName}; id = {id}";
        }
    }
    
    public class ItemData : BaseDataClass<Item>
    {
        private static ItemData _instance;

        private ItemData()
        {
            DATA_LOCATION = "./tarkovdata/items.en.json";
            Load(item =>
            {
                Console.WriteLine(item);
                return true;
            });
        }

        public static ItemData GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ItemData();
            }

            return _instance;
        }
    }
}