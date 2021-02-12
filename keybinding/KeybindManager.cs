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
    public class KeybindManager
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

        private static KeybindManager instance;

        public bool EnableBinds = true;

        public static KeybindManager GetInstance(bool testFile = false)
        {
            if (instance == null)
                instance = new KeybindManager(testFile);

            return instance;
        }

        private KeybindFileObject binds;

        private KeybindManager(bool testFile = false)
        {

            if (testFile)
                CONFIG_NAME = "keybinds.test.json";

            if (!File.Exists(CONFIG_PATH))
                File.Create(CONFIG_PATH).Close();

            LoadFromFile();
        }


        #region ModifyState

        private void LoadFromFile()
        {
            var json = File.ReadAllText(CONFIG_PATH);

            Debug.WriteLine("Found JSON: " + json);

            if (String.IsNullOrEmpty(json))
            {
                Debug.WriteLine("Empty Keybinds file found");
                binds = new KeybindFileObject(true);
                SaveToFile();
            }
            else
            {
                binds = JsonConvert.DeserializeObject<KeybindFileObject>(json);
            }
        }

        public void Reload()
        {
            LoadFromFile();
        }

        public void ResetToDefault()
        {
            binds = new KeybindFileObject(true);
            SaveToFile();
        }

        #endregion


        #region CRUD

        public void SaveToFile()
        {
            var json = JsonConvert.SerializeObject(binds);

            Debug.WriteLine("Writing JSON: " + json);

            File.WriteAllText(CONFIG_PATH, json);
        }

        public void SetKeybind(Keybind keybind, Key keyToBind)
        {
            Debug.WriteLine($"Writing Bind ({keybind}, {keyToBind})");


            // if this is null, then we don't have a duplicate
            Key? foundBind = GetKeyForKeybind(keybind);

            if (GetKeyForKeybind(keybind) != null)
            {
                Debug.WriteLine("Attempt to double-bind a keybind");

                // if a keybind bind already exists, remove then later add
                // to prevent duplicate binding
                binds.BindMap.Remove(foundBind.ToString());
            }

            binds.BindMap[keyToBind.ToString()] = keybind.ToString();
        }


        private void ClearBind(string key)
        {
            if (HasKeyBound(key))
            {
                Debug.WriteLine($"Unbinding key: {key}");
                binds.BindMap.Remove(key);
            }
        }
        public void ClearKey(Key key)
        {
            ClearBind(key.ToString());
        }

        public void ClearKeybind(Keybind keybind)
        {
            foreach (DictionaryEntry entry in binds.BindMap) {
                if (Enum.TryParse(entry.Value as string, true, out Keybind hk))
                {
                    // if we hit the keybind we need to clear
                    if (hk == keybind)
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

        private Keybind? GetKeybindForKey(string key)
        {

            // this will prevent us from activating binds whilst editing them
            if (!EnableBinds)
                return null;

            Keybind keybind = Keybind.None;

            // Debug.WriteLine($"Getting bind for {key}, raw output: ");

            if (HasKeyBound(key))
            {
                // Debug.WriteLine($"Key is bound, returning keybind for {key}");
                Key.TryParse(binds.BindMap[key] as string, true, out keybind);
            }

            if (keybind == Keybind.None)
                return null;

            return keybind;
        }

        // Overload for above
        public Keybind? GetKeybindForKey(Key key)
        {
            return GetKeybindForKey(key.ToString());
        }

        private bool HasKeyBound(string key)
        {

            string bindValue = "";

            bool bindIsEmpty = true;
            bool bindFound = binds.BindMap.Contains(key);

            if (bindFound)
            {
                bindValue = binds.BindMap[key] as string;
                bindIsEmpty = String.IsNullOrEmpty(bindValue);

                // if(!bindIsEmpty)
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


        public Key? GetKeyForKeybind(Keybind keybind)
        {
            foreach (DictionaryEntry entry in binds.BindMap)
            {
                if (entry.Value as string == keybind.ToString())
                {
                    if (Key.TryParse(entry.Key.ToString(), true, out Key parsedKey))
                    {
                        return parsedKey;
                    }
                }
            }

            return null;
        }

        #endregion




        public void Debug_WriteBindsDictionary()
        {
            if (binds.BindMap == null || binds.BindMap.Count <= 0)
            {
                return;
            }

            Debug.WriteLine($"---- Loaded Keybinds ----");

            foreach (DictionaryEntry entry in binds.BindMap)
            {
                Debug.WriteLine($"{entry.Key} - {entry.Value}");
            }

            Debug.WriteLine($"----                 ----");

        }
    }
}
