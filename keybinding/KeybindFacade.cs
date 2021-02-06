using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarkovAssistantWPF.keybinding
{
    class KeybindFacade
    {
        Hashtable keybinds = new Hashtable(new OrderedDictionary(16));

        KeybindFile keybindFile = new KeybindFile();

        public void LoadKeybindsToMap()
        {
            
        }
    }
}
