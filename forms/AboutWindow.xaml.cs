using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
