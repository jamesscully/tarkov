using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MenuItem = System.Windows.Controls.MenuItem;
using Point = System.Windows.Point;
using Size = System.Windows.Size;
using TextBox = System.Windows.Controls.TextBox;
using AutoUpdaterDotNET;
using Microsoft.Win32;
using TarkovAssistantWPF.enums;
using TarkovAssistantWPF.keybinding;

namespace TarkovAssistantWPF
{
    /// <summary>
    /// Handles the Map viewer functionality; this could be embedded into another main form later on,
    /// with the introduction of item searching
    /// </summary>

    public partial class MapWindow : Window
    {
        private IKeyboardMouseEvents keyHook;
        private bool _isGlobalKeysEnabled = false;

        const string userRoot = "HKEY_CURRENT_USER";
        const string subkey = "Tarkov Assistant";
        const string keyName = userRoot + "\\" + subkey;

        public MapWindow()
        {
            Debug.WriteLine("################\n");
            Debug.WriteLine($"Launched, Assembly Version {Assembly.GetEntryAssembly().GetName().Version}\n ");
            Debug.WriteLine("################\n");

            InitializeComponent();

            AutoUpdater.RunUpdateAsAdmin = false;

            // check our remote update file, see if we need an update!
            AutoUpdater.Start(Properties.Resources.update_xml_url);


            Debug.WriteLine(HotkeyEnum.CYCLE_MAP.ToString());

            keyHook = Hook.GlobalEvents();
            keyHook.KeyUp += KeyHookOnKeyUp;

            // map menu buttons to needed functions
            menuBar.OnMapButtonPress += mapControl.SetMap;
            menuBar.OnGlobalHotkeyToggle += enableHotkeys => _isGlobalKeysEnabled = enableHotkeys;

            bool enabled = ((int) Registry.GetValue(keyName, "EnableGlobalHotkeys", 0)) == 1;

            _isGlobalKeysEnabled = enabled;

            var fac = new KeybindFacade();
        }

        private void KeyHookOnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!_isGlobalKeysEnabled)
                return;

            Debug.WriteLine(e.KeyCode);

            switch (e.KeyCode)
            {
                case Keys.NumPad9:
                    mapControl.CycleSubMap();
                    break;
            }

        }


        private bool _fullscreen = false;

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // fullscreen
            if (e.Key == Key.F || e.Key == Key.F11)
            {
                this.WindowState = (_fullscreen) ? WindowState.Normal : WindowState.Maximized;
                this.WindowStyle = (_fullscreen) ? WindowStyle.SingleBorderWindow : WindowStyle.None;
                _fullscreen = !_fullscreen;
            }

            if (e.Key == Key.C)
            {
                mapControl.ClearCanvas();
            }

            if (e.Key == Key.R)
            {
                mapControl.ResetTransforms();
            }
        }
    }
}
