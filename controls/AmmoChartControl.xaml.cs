using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TarkovAssistantWPF.controls
{
    public partial class AmmoChartControl : UserControl
    {
        private bool _showBoundingBox = true;
        public AmmoChartControl()
        {
            InitializeComponent();
            
        }
        
        private void AmmoCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Console.WriteLine("Resized! New: " + e.NewSize + " Old: " + e.PreviousSize);
            Console.WriteLine("Canvas Sizes: ");
            Console.WriteLine("Height: " + ammoCanvas.Height + " Width: " + ammoCanvas.Width);
            Console.WriteLine("Height: " + ammoCanvas.ActualHeight + " Width: " + ammoCanvas.ActualWidth);
            
            ammoCanvas.Children.Clear();
            DrawGrid();

        }

        private void DrawBoundingBox()
        {
            Rectangle rectangle = new Rectangle();
            
            rectangle.Stroke = Brushes.Magenta;
            rectangle.StrokeThickness = 5;
            rectangle.Width = ammoCanvas.RenderSize.Width;
            rectangle.Height = ammoCanvas.RenderSize.Height;

            ammoCanvas.Children.Add(rectangle);
        }

        private void DrawGrid()
        {
            
            if (_showBoundingBox)
            {
                DrawBoundingBox();
            }
            
            var h = ammoCanvas.ActualHeight;
            var w = ammoCanvas.ActualWidth;
            
            Console.WriteLine("Drawing grid with height of " + h + " and width of " + w);

            if (h == 0 || w == 0)
            {
                return;
            }

            DrawGridYLines();
            DrawMainLines();
            DrawGridDamageLines();
        }

        private void DrawMainLines()
        {
            Polyline line = new Polyline();

            line.Stroke = Brushes.DimGray;
            line.StrokeThickness = 6;

            Point topLeft = new Point(0, 0);
            Point bottomLeft = new Point(0, ammoCanvas.ActualHeight);
            Point bottomRight = new Point(ammoCanvas.ActualWidth, ammoCanvas.ActualHeight);
            
            line.Points.Add(topLeft);
            line.Points.Add(bottomLeft);
            line.Points.Add(bottomRight);

            ammoCanvas.Children.Add(line);
            

            TextBlock damageHeader = new TextBlock
            {
                Text = "DAMAGE",
                FontWeight = FontWeights.ExtraBold,
                Foreground = Brushes.White
            };
            
            Canvas.SetTop(damageHeader, ammoCanvas.ActualHeight + 10);

            ammoCanvas.Children.Add(damageHeader);
            
            TextBlock penetrationHeader = new TextBlock
            {
                Text = "PENETRATION",
                FontWeight = FontWeights.ExtraBold,
                Foreground = Brushes.White,
            };

            Canvas.SetLeft(penetrationHeader, 0 - 25);
            Canvas.SetTop(penetrationHeader, ammoCanvas.ActualHeight);

            penetrationHeader.RenderTransform = new RotateTransform(270);

            ammoCanvas.Children.Add(penetrationHeader);


        }
        private void DrawGridYLines()
        {
            
            var h = ammoCanvas.ActualHeight;
            var w = ammoCanvas.ActualWidth;
            
            
            var lineCount = 7;

            var interval = h / lineCount;

            var index = 6;

            for (double y = interval; y < h; y += interval)
            {
                Line line = new Line
                {
                    Stroke = Brushes.DarkGray,
                    StrokeThickness = 1,
                    VerticalAlignment = VerticalAlignment.Center
                };

                line.X1 = 0;
                line.X2 = w - 75;
                
                
                if(index != 0)
                    DrawArmorLabel("Class " + index, w - 50, y - 7);

                line.Y1 = y;
                line.Y2 = y;

                ammoCanvas.Children.Add(line);

                index--;
            }

        }

        private void DrawArmorLabel(string text, double x, double y)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center
            };

            
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);

            ammoCanvas.Children.Add(textBlock);
        }

        private void DrawGridDamageLines()
        {
            double maxDamage = 260;

            double interval = (ammoCanvas.ActualWidth / maxDamage) * 20;

            int index = 0;

            for (double x = 0; x < ammoCanvas.ActualWidth; x += interval)
            {
                Line tick = new Line
                {
                    Stroke = Brushes.Green,
                    StrokeThickness = 1
                };

                tick.X1 = x;
                tick.X2 = x;

                tick.Y1 = ammoCanvas.ActualHeight;
                tick.Y2 = ammoCanvas.ActualHeight - 10;

                TextBlock value = new TextBlock();
                value.Text = ((x / interval) * 20).ToString();
                
                Canvas.SetLeft(value, x);
                Canvas.SetTop(value, (ammoCanvas.ActualHeight - 40));

                // value.RenderTransform = new RotateTransform(270);

                ammoCanvas.Children.Add(tick);
                ammoCanvas.Children.Add(value);

                index++;
            }
            
        }
    }
}