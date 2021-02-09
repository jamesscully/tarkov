using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TarkovAssistantWPF.enums;

namespace TarkovAssistantWPF.keybinding
{
    class KeybindFacade
    {
        Hashtable keybinds = new Hashtable(new OrderedDictionary(16));

        KeybindFile keybindFile = KeybindFile.GetInstance();

        public KeybindFacade()
        {
            LoadKeybindsToMap();
        }

        public void LoadKeybindsToMap()
        {
            keybinds = new Hashtable(keybindFile.GetHotkeyMap());
        }

        public bool CheckIfHotkey(Key key)
        {
            return keybinds.Contains(key.ToString());
        }

        public HotkeyEnum GetHotkeyFromKey(Key key)
        {
            if (!CheckIfHotkey(key))
            {
                return HotkeyEnum.NO_OP;
            }

            return (HotkeyEnum) keybinds[key.ToString()];
        }
    }
}
