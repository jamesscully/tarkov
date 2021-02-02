﻿using Gma.System.MouseKeyHook;
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

        public MapWindow()
        {
            Debug.WriteLine("################\n");
            Debug.WriteLine($"Launched, Assembly Version {Assembly.GetEntryAssembly().GetName().Version}\n ");
            Debug.WriteLine("################\n");

            InitializeComponent();

            // check our remote update file, see if we need an update!
            AutoUpdater.Start(Properties.Resources.update_xml_url);

            keyHook = Hook.GlobalEvents();
            keyHook.KeyPress += GlobalHookKeyPress;

            // map menu buttons to needed functions
            menuBar.OnMapButtonPress += mapControl.SetMap;
            menuBar.OnGlobalHotkeyToggle += enableHotkeys => _isGlobalKeysEnabled = enableHotkeys;
        }

        private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {
            // do nothing if the user does not want global keys enabled
            if (!_isGlobalKeysEnabled)
                return;

            Debug.WriteLine("Global KeyPress: " + e.KeyChar);

            if (e.KeyChar == '9')
            {
                mapControl.CycleSubMap();
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
