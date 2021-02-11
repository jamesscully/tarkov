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
            Binds.Add("NumPad4", HotkeyEnum.PAN_LEFT.ToString());
            Binds.Add("NumPad6", HotkeyEnum.PAN_RIGHT.ToString());
            Binds.Add("NumPad8", HotkeyEnum.PAN_UP.ToString());
            Binds.Add("NumPad2", HotkeyEnum.PAN_DOWN.ToString());
            Binds.Add("C", HotkeyEnum.CLEAR.ToString());
            Binds.Add("Subtract", HotkeyEnum.ZOOM_OUT.ToString());
            Binds.Add("Add", HotkeyEnum.ZOOM_IN.ToString());
            Binds.Add("Multiply", HotkeyEnum.CYCLE_MAP.ToString());
        }

    }
}
