using System;
using System.Collections.Generic;
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
using TarkovAssistantWPF.data;

namespace TarkovAssistantWPF.controls
{
    /// <summary>
    /// Interaction logic for AmmoChartControl.xaml
    /// </summary>
    public partial class AmmoPageControl : UserControl
    {
        private string selectedCaliber = "";
        
        public AmmoPageControl()
        {
            InitializeComponent();
            
            AmmoData data = AmmoData.GetInstance();

            foreach (string c in data.GetAllCalibers())
            {
                Button text = new Button();

                text.Content = AmmoData.NormalizeCaliberName(c);
                
                text.FontWeight = FontWeights.Bold;
                text.Margin = new Thickness(6, 0, 6, 0);

                text.Click += (sender, args) =>
                {
                    ammoChart.SetCaliber(c);
                };

                ammoTypeContainer.Children.Add(text);
            }
            
        }
    }
}
