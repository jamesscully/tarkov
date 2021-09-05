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

            if (_showBoundingBox)
            {
                DrawBoundingBox();
            }
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
            rectangle.StrokeThickness = 50;
            rectangle.Width = ammoCanvas.RenderSize.Width;
            rectangle.Height = ammoCanvas.RenderSize.Height;

            ammoCanvas.Children.Add(rectangle);
        }

        private void DrawGrid()
        {
            var h = ammoCanvas.ActualHeight;
            var w = ammoCanvas.ActualWidth;
            
            Console.WriteLine("Drawing grid with height of " + h + " and width of " + w);

            if (h == 0 || w == 0)
            {
                return;
            }

            DrawGridYLines();
            DrawMainLines();
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

        private void DrawGridXLines()
        {
            
        }


    }
}