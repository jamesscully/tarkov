using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Newtonsoft.Json;
using TarkovAssistantWPF.enums;

namespace TarkovAssistantWPF.keybinding
{
    public class JsonKeybinds
    {
        private XDocument config;

        private static string CONFIG_FOLDER = "config/";
        private static string CONFIG_NAME = "keybinds.json";

        private static string CONFIG_PATH
        {
            get
            {
                return CONFIG_FOLDER + CONFIG_NAME;
            }
        }

        private static JsonKeybinds instance;

        public static JsonKeybinds GetInstance(bool testFile = false)
        {
            if (instance == null)
                instance = new JsonKeybinds(testFile);

            return instance;
        }

        private JsonKeybindsObject binds;

        private JsonKeybinds(bool testFile = false)
        {

            if (testFile)
                CONFIG_NAME = "keybinds.test.json";

            if (!File.Exists(CONFIG_PATH))
                File.Create(CONFIG_PATH).Close();

            LoadBinds();
        }


        #region ModifyState

        private void LoadBinds()
        {
            var json = File.ReadAllText(CONFIG_PATH);

            Debug.WriteLine("Found JSON: " + json);

            if (String.IsNullOrEmpty(json))
            {
                Debug.WriteLine("Empty Keybinds file found");
                binds = new JsonKeybindsObject(true);
            }
            else
            {
                binds = JsonConvert.DeserializeObject<JsonKeybindsObject>(json);
            }
        }

        public void Reload()
        {
            LoadBinds();
        }

        public void ResetToDefault()
        {
            binds = new JsonKeybindsObject(true);
            SaveBinds();
        }

        #endregion


        #region CRUD

        public void SaveBinds()
        {
            var json = JsonConvert.SerializeObject(binds);

            Debug.WriteLine("Writing JSON: " + json);

            File.WriteAllText(CONFIG_PATH, json);
        }

        public void SetBind(HotkeyEnum hotkey, Key keyToBind)
        {
            Debug.WriteLine($"Writing Bind ({hotkey}, {keyToBind})");
            binds.Binds[keyToBind.ToString()] = hotkey.ToString();
        }


        public void ClearBind(string key)
        {
            if (HasKeyBound(key))
            {
                Debug.WriteLine($"Unbinding key: {key}");
                binds.Binds.Remove(key);
            }
        }
        public void ClearBind(Key key)
        {
            ClearBind(key.ToString());
        }

        public void ClearBind(HotkeyEnum hotkey)
        {
            foreach (DictionaryEntry entry in binds.Binds) {
                if (Enum.TryParse(entry.Value as string, true, out HotkeyEnum hk))
                {
                    // if we hit the hotkey we need to clear
                    if (hk == hotkey)
                    {
                        // then wipe and return
                        ClearBind(entry.Key as string);
                        return;

                    }
                }
            }
        }

        #endregion


        #region Accessors

        public HotkeyEnum? GetHotkeyForBind(string key)
        {
            HotkeyEnum hotkey = HotkeyEnum.NO_OP;

            // Debug.WriteLine($"Getting bind for {key}, raw output: ");

            if (HasKeyBound(key))
            {
                // Debug.WriteLine($"Key is bound, returning hotkey for {key}");
                Key.TryParse(binds.Binds[key] as string, true, out hotkey);
            }

            if (hotkey == HotkeyEnum.NO_OP)
                return null;

            return hotkey;
        }

        // Overload for above
        public HotkeyEnum? GetHotkeyForBind(Key key)
        {
            return GetHotkeyForBind(key.ToString());
        }

        public bool HasKeyBound(string key)
        {

            string bindValue = "";

            bool bindIsEmpty = true;
            bool bindFound = binds.Binds.Contains(key);

            if (bindFound)
            {
                bindValue = binds.Binds[key] as string;
                bindIsEmpty = String.IsNullOrEmpty(bindValue);

                if(!bindIsEmpty)
                    // Debug.WriteLine($"Found bind ({bindValue}) for key {key}");
            }

            // Debug.WriteLine($"HasKeyBound: {bindValue} {key}, {(bindFound && !bindIsEmpty)} ");

            return (bindFound && !bindIsEmpty);
        }

        // Overload for above
        public bool HasKeyBound(Key key)
        {
            return HasKeyBound(key.ToString());
        }

        #endregion




        public void Debug_WriteBindsDictionary()
        {
            if (binds.Binds == null || binds.Binds.Count <= 0)
            {
                return;
            }

            Debug.WriteLine($"---- Loaded Keybinds ----");

            foreach (DictionaryEntry entry in binds.Binds)
            {
                Debug.WriteLine($"{entry.Key} - {entry.Value}");
            }

            Debug.WriteLine($"----                 ----");

        }
    }
}
