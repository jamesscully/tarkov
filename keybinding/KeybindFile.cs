using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public KeybindFile()
        {
            Debug.WriteLine("Loading keybinds config");

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
                    WriteDefaultConfig();
            }
            finally
            {
                config = XDocument.Load(CONFIG_PATH);
            }

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
                writer.WriteStartElement("keybinds");

                writer.WriteElementString(HotkeyEnum.CYCLE_MAP.ToString(), "");
                writer.WriteElementString(HotkeyEnum.CYCLE_SUB_MAP.ToString(), "NumPad9");
                writer.WriteElementString(HotkeyEnum.NEXT_MAP.ToString(), "");
                writer.WriteElementString(HotkeyEnum.PREV_MAP.ToString(), "");
                writer.WriteElementString(HotkeyEnum.RESET.ToString(), "R");
                writer.WriteElementString(HotkeyEnum.ZOOM_IN.ToString(), "");
                writer.WriteElementString(HotkeyEnum.ZOOM_OUT.ToString(), "");

                writer.WriteEndElement();
            }
        }
    }
}
