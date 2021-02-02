﻿using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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
            InitializeComponent();

            keyHook = Hook.GlobalEvents();

            keyHook.KeyPress += GlobalHookKeyPress;

            menuItem_EnableGlobalKeys.IsChecked = _isGlobalKeysEnabled;

            // set search bars hint
            // quickSearch.Text = Properties.Resources.str_searchhint;
        }

        private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {
            // do nothing if the user does not want global keys enabled
            if (!_isGlobalKeysEnabled)
                return;
        }

        private bool _fullscreen = false;

        // Fired when a map button is pressed
        private void OnMapChange(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;

            Map.TryParse((item.Tag as string), true, out Map map);

            mapControl.SetMap(map);

            Debug.WriteLine(menuBar.Items.Count);

            if (map == Map.RESERVE && menuBar.Items.Count == 1)
            {

                MenuItem TunnelMap = new MenuItem();
                TunnelMap.Header = "Bunker View";
                TunnelMap.Click += (o, args) => mapControl.SetMap(Map.RESERVE_TUNNELS);

                MenuItem GroundMap = new MenuItem();
                GroundMap.Header = "Ground View";
                GroundMap.Click += (o, args) => mapControl.SetMap(Map.RESERVE);

                menuBar.Items.Add(TunnelMap);
                menuBar.Items.Add(GroundMap);
            }
            else
            {
                if (menuBar.Items.Count > 1)
                {
                    var mapMenuItem = menuBar.Items.GetItemAt(0);

                    // remove-all and re-add 
                    menuBar.Items.Clear();
                    menuBar.Items.Add(mapMenuItem);

                    menuBar.Items.Refresh();
                }
            }
        }


        #region SearchFunctions

        private void QuickSearch_OnKeyUp(object sender, KeyEventArgs e)
        {
            // if (e.Key == Key.Enter)
            // {
            //
            // }
            //
            // if (quickSearch.Text.Length < 3 || e.Key == Key.Back)
            //     return;
            //
            // Debug.WriteLine($"Searching for pages: {(sender as TextBox).Text}");
            //
            //
            // string api_url =
            //     $"https://escapefromtarkov.gamepedia.com/api.php?action=opensearch&format=json&formatversion=2&search={quickSearch.Text}&namespace=0&limit=3&suggest=true";
            //
            //
            // Task.Run(() =>
            // {
            //     WebRequest request = HttpWebRequest.Create(api_url);
            //     WebResponse response = request.GetResponse();
            //
            //     StreamReader reader = new StreamReader(response.GetResponseStream());
            //
            //     string json = reader.ReadToEndAsync().Result;
            //
            //     Debug.WriteLine(json);
            // });
        }

        private void QuickSearch_OnIsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            bool focused = e.NewValue.ToString() == "True";

            TextBox textbox = (TextBox) sender;

            if (focused)
            {
                textbox.Foreground = Brushes.Black;
                textbox.Text = "";
            }
            else
            {
                textbox.Foreground = Brushes.Gray;
                textbox.Text = Properties.Resources.str_searchhint;
            }
        }

        #endregion

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

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(menuItem_EnableGlobalKeys))
            {
                MenuItem item = (MenuItem) sender;
                bool isChecked = !item.IsChecked;

                // update var and ui
                _isGlobalKeysEnabled = isChecked;
                menuItem_EnableGlobalKeys.IsChecked = isChecked;
            }
        }
    }
}
