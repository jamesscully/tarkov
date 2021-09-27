using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace TarkovAssistantWPF.forms
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            try
            {
                string buildVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
                aboutTxtBuildVersion.Content = "Build Version " + buildVersion;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            
        }

        private void OnDeactivated(object sender, KeyboardFocusChangedEventArgs e)
        {
            var window = (Window)sender;
            window.Topmost = true;
            window.Activate();
        }
    }
}
