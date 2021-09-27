using System.Collections.Specialized;
using TarkovAssistantWPF.enums;

namespace TarkovAssistantWPF.keybinding
{
    public class KeybindFileObject 
    {

        public OrderedDictionary BindMap = new OrderedDictionary();
        public KeybindFileObject(bool loadDefault = false)
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
