using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TarkovAssistantWPF.data.models;

namespace TarkovAssistantWPF.data
{
    
    public class TraderData : BaseDataClass<Trader>
    {
        private TraderData _instance;
        
        private TraderData()
        {
            DATA_LOCATION = Constants.DATA_LOCATION_TRADERS;
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