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
using TarkovAssistantWPF.interfaces;
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

            // check our remote update file, see if we need an update!
            AutoUpdater.Start(Properties.Resources.update_xml_url);


            Debug.WriteLine(Keybind.CycleMap.ToString());

            keyHook = Hook.GlobalEvents();
            keyHook.KeyUp += KeyHookOnKeyUp;

            // map menu buttons to needed functions
            menuBar.OnMapButtonPress += mapControl.SetMap;
            menuBar.OnGlobalHotkeyToggle += enableHotkeys => _isGlobalKeysEnabled = enableHotkeys;

            bool enabled = ((int) Registry.GetValue(keyName, "EnableGlobalHotkeys", 0)) == 1;

            _isGlobalKeysEnabled = enabled;
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

            if (KeybindManager.GetInstance().HasKeyBound(e.Key))
            {
                var keybind = KeybindManager.GetInstance().GetKeybindForKey(e.Key);

                if (keybind == Keybind.Reset)
                {
                    mapControl.OnReset();
                }

                if (keybind == Keybind.Clear)
                {
                    mapControl.OnClear();
                }

            }
        }

        private void KeyHookOnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!_isGlobalKeysEnabled || !KeybindManager.GetInstance().EnableBinds)
                return;

            // ignore if we can't parse it
            if (!Keys.TryParse(e.KeyCode.ToString(), true, out Key key))
            {
                return;
            }

            IHotkeyAble[] controls = { mapControl };

            Keybind? hotkeyAction = Keybind.None;

            if (KeybindManager.GetInstance().HasKeyBound(key))
            {
                hotkeyAction = KeybindManager.GetInstance().GetKeybindForKey(key);

                foreach(IHotkeyAble x in controls)
                {
                    switch (hotkeyAction)
                    {

                        // Map selection
                        case Keybind.CycleSubMap:
                            x.OnCycleSubMap();
                            break;

                        case Keybind.CycleMap:
                            x.OnCycleMap();
                            break;

                        case Keybind.NextMap:
                            x.OnNextMap();
                            break;

                        case Keybind.PrevMap:
                            x.OnPrevMap();
                            break;

                        // Zoom
                        case Keybind.ZoomIn:
                            x.OnZoomIn();
                            break;

                        case Keybind.ZoomOut:
                            x.OnZoomOut();
                            break;

                        // Panning
                        case Keybind.PanLeft:
                            x.OnPan(1, 0);
                            break;

                        case Keybind.PanRight:
                            x.OnPan(-1, 0);
                            break;

                        case Keybind.PanUp:
                            x.OnPan(0, 1);
                            break;

                        case Keybind.PanDown:
                            x.OnPan(0, -1);
                            break;

                        // Do nothing (no operation)
                        case Keybind.None: 
                            break;
                        
                        // these should not be handled when coming from outside the form;
                        // they are commonly bound in-game by default
                        case Keybind.Clear:
                        case Keybind.Reset: 
                            break;

                        // only map commands should be left now
                        default:

                            // enum is format of SETMAP_(MAPNAME), so use after _

                            try
                            {
                                var mapName = hotkeyAction.ToString().Split('_')[1];
                                if (Enum.TryParse(mapName, true, out Map mapToSet))
                                {
                                    x.OnSetMap(mapToSet);
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                            break;
                    }
                }
            }
        }
    }
}
