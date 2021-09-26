using System;

namespace TarkovAssistantWPF.data
{

    public class HideoutStation
    {
        public int id = -1;
        public string function = "";
        public string imageSrc = "";
    }
    
    public class Module
    {
        public class Requirement
        {
            enum RequirementTypes
            {
                ITEM, TRADER, MODULE
            }
        }
        
        public string module = "";
        public int level = -1;
        
    }

    
    public class HideoutData : BaseDataClass<HideoutStation>
    {
        private HideoutData _instance;

        private HideoutData()
        {
            
        }

        public HideoutData GetInstance()
        {
            if (_instance == null)
            {
                _instance = new HideoutData();
            }

            return _instance;
        }

        public override void Load(Func<HideoutStation, bool> forEachHook)
        {
            
            base.Load(forEachHook);
        }
    }
}