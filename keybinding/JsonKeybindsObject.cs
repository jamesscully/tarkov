using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using TarkovAssistantWPF.enums;

namespace TarkovAssistantWPF.keybinding
{
    public class JsonKeybindsObject 
    {

        public OrderedDictionary Binds = new OrderedDictionary();
        public JsonKeybindsObject(bool loadDefault = false)
        {
            if(loadDefault)
                SetDefaults();
        }

        public void SetDefaults()
        {
            Binds.Add("NumPad9", HotkeyEnum.CYCLE_SUB_MAP.ToString());
            Binds.Add("R", HotkeyEnum.RESET.ToString());
        }

    }
}
