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
                
                text.Margin = new Thickness(6, 6, 6, 6);
                text.Padding = new Thickness(6, 0, 6, 0);

                text.Click += (sender, args) =>
                {
                    var button = sender as Button;
                    

                    bool enabled = false;
                    bool uninitialized = false;
                    try
                    {
                        enabled = (bool) button.Tag;
                    }
                    catch (NullReferenceException e)
                    {
                        // if we have a null tag, then we haven't yet enabled this button
                        enabled = true;
                        uninitialized = true;
                        button.Tag = true;
                        button.Background = Brushes.Green;
                        ammoChart.AddCaliber(c);

                    }

                    if (enabled && !uninitialized)
                    {
                        ammoChart.RemoveCaliber(c);

                        button.Tag = false;
                        button.ClearValue(Button.BackgroundProperty);
                    }
                    else if (!enabled && !uninitialized)
                    {
                        ammoChart.AddCaliber(c);

                        button.Tag = true;
                        button.Background = Brushes.Green;
                    }
                };

                ammoTypeContainer.Children.Add(text);
            }

            ammoTypeContainer.HorizontalAlignment = HorizontalAlignment.Center;

        }
    }
}
