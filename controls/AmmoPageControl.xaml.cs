﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TarkovAssistantWPF.data;

namespace TarkovAssistantWPF.controls
{
    /// <summary>
    /// Interaction logic for AmmoChartControl.xaml
    /// </summary>
    public partial class AmmoPageControl : UserControl
    {

        public AmmoPageControl()
        {
            InitializeComponent();
            
            AmmoData data = AmmoData.GetInstance();

            foreach (string c in data.GetAllCalibers())
            {
                Button caliberButton = new Button();

                caliberButton.Content = AmmoData.NormalizeCaliberName(c);

                caliberButton.Margin = new Thickness(6, 6, 6, 6);
                caliberButton.Padding = new Thickness(6, 6, 6, 6);

                caliberButton.Click += (sender, args) =>
                {
                    var button = sender as Button;
                    bool enabled = ammoChart.HasCaliber(c);
                    
                    if (enabled)
                    {
                        ammoChart.RemoveCaliber(c);
                        button.ClearValue(Button.BackgroundProperty);
                    }
                    else
                    {
                        ammoChart.AddCaliber(c);
                        button.Background = Brushes.Green;
                    }
                };

                ammoTypeContainer.Children.Add(caliberButton);
            }

            ammoTypeContainer.HorizontalAlignment = HorizontalAlignment.Center;

        }
    }
}
