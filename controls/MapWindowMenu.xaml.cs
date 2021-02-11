using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoUpdaterDotNET;
using Microsoft.Win32;
using TarkovAssistantWPF.forms;

namespace TarkovAssistantWPF.controls
{
    /// <summary>
    /// Interaction logic for MapWindowMenu.xaml
    /// </summary>
    ///
    
    public delegate void OnMapButtonPress(Map map);
    public delegate void OnGlobalHotkeysChanged(bool enableHotkeys);

    public partial class MapWindowMenu : UserControl
    {
        public event OnMapButtonPress OnMapButtonPress;
        public event OnGlobalHotkeysChanged OnGlobalHotkeyToggle;

        const string userRoot = "HKEY_CURRENT_USER";
        const string subkey = "Tarkov Assistant";
        const string keyName = userRoot + "\\" + subkey;

        public MapWindowMenu()
        {
            InitializeComponent();

            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            //todo: refactor reg. values to a separate class
            bool enabled = ((int) Registry.GetValue(keyName, "EnableGlobalHotkeys", 0)) == 1;

            menuItem_EnableGlobalKeys.IsChecked = enabled;
        }

        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            Debug.WriteLine("\n\n Checking for update \n\n");

            if (args.IsUpdateAvailable)
            {
                Debug.WriteLine("\n\n FOUND UPDATE \n\n");
                menuItem_UpdateAvailableButton.Header = "Update Available!";
                menuItem_UpdateAvailableButton.Visibility = Visibility.Visible;

                menuItem_UpdateAvailableButton.Click += (s, e) =>
                {
                    var result = MessageBox.Show(
                        $"A new update ({args.CurrentVersion}) is available.\n\nDo you wish to update now? (requires restart)",
                        "Update",
                        MessageBoxButton.YesNoCancel, 
                        MessageBoxImage.Question
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        if (AutoUpdater.DownloadUpdate(args))
                        {
                            Application.Current.Shutdown();
                        }
                    }
                };
            }
        }


        protected virtual void MapButtonPressed(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem) sender;

            Map.TryParse((item.Tag as string), true, out Map map);

            OnMapButtonPress?.Invoke(map);
        }

        private void ToggleGlobalHotkeys(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem) sender;

            item.IsChecked = !item.IsChecked;

            if(item.IsChecked)
                Registry.SetValue(keyName, "EnableGlobalHotkeys", 1);
            else
                Registry.SetValue(keyName, "EnableGlobalHotkeys", 0);

            Debug.WriteLine("Registry Value: " + Registry.GetValue(keyName, "EnableGlobalHotkeys", 0));


            OnGlobalHotkeyToggle?.Invoke(item.IsChecked);
        }

        private void SetUpdateAvailable(bool available)
        {
            if (available)
            {
                menuItem_UpdateAvailableButton.Header = "Update available!";
            }
        }


        private void ShowHotkeysWindow(object sender, RoutedEventArgs e)
        {
            var window = new ChangeHotkeysWindow();
            window.Show();
        }
    }
}
