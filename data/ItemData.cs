using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TarkovAssistantWPF.data.models;
using TarkovAssistantWPF.interfaces;

namespace TarkovAssistantWPF.data
{
    public class ItemData : BaseDataClass<Item>
    {
        private static ItemData _instance;

        private ItemData()
        {
            DATA_LOCATION = Constants.DATA_LOCATION_ITEMS;
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