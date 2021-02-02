using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace TarkovAssistantWPF.controls
{
    /// <summary>
    /// Interaction logic for MapWindowMenu.xaml
    /// </summary>
    ///
    
    public delegate void OnMapButtonPress(Map map);
    public partial class MapWindowMenu : UserControl
    {
        public event OnMapButtonPress OnMapButtonPress;


        public MapWindowMenu()
        {
            InitializeComponent();
        }


        protected virtual void MapButtonPressed(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem) sender;

            Map.TryParse((item.Tag as string), true, out Map map);

            OnMapButtonPress?.Invoke(map);
        }

        private void MenuItem_EnableGlobalKeys_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }


    }
}
