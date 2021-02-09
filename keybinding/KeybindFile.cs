using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using TarkovAssistantWPF.enums;

namespace TarkovAssistantWPF.keybinding
{
    class KeybindFile
    {
        private XDocument config;

        private const string CONFIG_FOLDER = "config/";
        private const string CONFIG_NAME = "keybinds.xml";

        private const string CONFIG_PATH = CONFIG_FOLDER + CONFIG_NAME;

        private static KeybindFile instance;

        private bool modified = true;

        private OrderedDictionary keyMap;
        private OrderedDictionary changesMap;


        private KeybindFile()
        {
            Debug.WriteLine("****\n\n Loading keybinds config");

            if (!File.Exists(CONFIG_PATH))
            {
                WriteDefaultConfig();
            }

            try
            {
                config = XDocument.Load(CONFIG_PATH);
            }
            catch (XmlException e)
            {
                var result = MessageBox.Show(
                    "There was an error processing the keybinds file. Would you like to revert to default?",
                    "Error",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    WriteDefaultConfig();
                    config = XDocument.Load(CONFIG_PATH);
                }
            }
            finally
            {
                GetHotkeyMap();
            }

        }

        public static KeybindFile GetInstance()
        {
            if (instance == null)
            {
                instance = new KeybindFile();
            }

            return instance;
        }

        public OrderedDictionary GetHotkeyMap()
        {
            // if we already have a map (not null),
            // and we haven't modified, then directly return the map
            if (!modified || !(keyMap == null))
                return keyMap;

            using (XmlReader reader = XmlReader.Create(CONFIG_PATH))
            {
                reader.IsStartElement();

                while (reader.MoveToNextAttribute())
                {
                    if (reader.IsEmptyElement)
                        break;

                    if (Enum.TryParse(reader.LocalName, true, out HotkeyEnum keybind))
                    {
                        string value = reader.Value;

                        // do nothing if unbound
                        if (value == "")
                            continue;


                        Debug.WriteLine($"Found keybind: {reader.LocalName}, {reader.Value}");

                        // use button as the key, so that OnKeyDown queries are O(1)
                        keyMap.Add(value, keybind);
                    }
                }
            }

            changesMap = keyMap;

            Debug.WriteLine("KeybindFile: Returning keyMap " + keyMap, "Info");
            return keyMap;
        }

        public void GetKeyForBind(HotkeyEnum hotkey)
        {
            if (modified)
            {

            }
            else
            {

            }
        }

        public void CommitHotkey(HotkeyEnum hotkey, Key key)
        {
            modified = true;

            if (changesMap == null)
            {
                throw new Exception("Unable to commit change; map is null");
            }

            changesMap[key.ToString()] = hotkey;
        }

        public void CancelChanges()
        {
            modified = false;
            changesMap = null;
        }

        public void SaveChanges()
        {
            if (changesMap == null)
            {
                throw new Exception("Unable to write changes; map is null");
            }


            foreach (var pair in changesMap)
            {
                Debug.WriteLine(pair);
            }
        }

        public void WriteConfig()
        {
            
        }

        public static void WriteDefaultConfig()
        {
            if (!Directory.Exists(CONFIG_FOLDER))
            {
                Directory.CreateDirectory(CONFIG_FOLDER);
                File.Create(CONFIG_PATH);
            }


            using (XmlWriter writer = XmlWriter.Create(CONFIG_PATH))
            {
                writer.WriteStartElement("Keybinds");

                writer.WriteAttributeString(HotkeyEnum.CYCLE_MAP.ToString(), "");
                writer.WriteAttributeString(HotkeyEnum.CYCLE_SUB_MAP.ToString(), "NumPad9");
                writer.WriteAttributeString(HotkeyEnum.NEXT_MAP.ToString(), "");
                writer.WriteAttributeString(HotkeyEnum.PREV_MAP.ToString(), "");
                writer.WriteAttributeString(HotkeyEnum.RESET.ToString(), "R");
                writer.WriteAttributeString(HotkeyEnum.ZOOM_IN.ToString(), "");
                writer.WriteAttributeString(HotkeyEnum.ZOOM_OUT.ToString(), "");

                writer.WriteEndElement();
            }
        }
    }

    
}
