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

        public OrderedDictionary BindMap = new OrderedDictionary();
        public JsonKeybindsObject(bool loadDefault = false)
        {
            if(loadDefault)
                SetDefaults();
        }

        public void SetDefaults()
        {
            BindMap.Add("NumPad9", Keybind.CycleSubMap.ToString());
            BindMap.Add("R", Keybind.Reset.ToString());
            BindMap.Add("NumPad4", Keybind.PanLeft.ToString());
            BindMap.Add("NumPad6", Keybind.PanRight.ToString());
            BindMap.Add("NumPad8", Keybind.PanUp.ToString());
            BindMap.Add("NumPad2", Keybind.PanDown.ToString());
            BindMap.Add("C", Keybind.Clear.ToString());
            BindMap.Add("Subtract", Keybind.ZoomOut.ToString());
            BindMap.Add("Add", Keybind.ZoomIn.ToString());
            BindMap.Add("Multiply", Keybind.CycleMap.ToString());
        }

    }
}
