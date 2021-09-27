using System;
using TarkovAssistantWPF.data.models;

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