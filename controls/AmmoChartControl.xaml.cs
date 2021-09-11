using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TarkovAssistantWPF.data;

namespace TarkovAssistantWPF.controls
{
    public partial class AmmoChartControl : UserControl
    {
        private bool _showBoundingBox = false;

        private HashSet<string> _selectedCalibers = new HashSet<string>();

        private double maxDamage = 260;
        
        
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
            
            ResetCanvas();
        }

        private void ResetCanvas()
        {
            ammoCanvas.Children.Clear();
            DrawGrid();
            DrawCaliberDataPoints();
        }

        public void AddCaliber(string caliber)
        {
            _selectedCalibers.Add(caliber);
            ResetCanvas();
        }
        
        public void RemoveCaliber(string caliber)
        {
            _selectedCalibers.Remove(caliber);
            ResetCanvas();
        }

        private Color GetRandomColour()
        {
            var color = new Color();
            var rand = new Random();

            byte[] bytes = new byte[3];
            
            rand.NextBytes(bytes);
            
            Console.WriteLine($"Random colour: {bytes[0]} {bytes[1]} {bytes[2]}" );


            color.A = 255; color.R = bytes[0]; color.G = bytes[1]; color.B = bytes[2];

            return color;
        }

        private void DrawCaliberDataPoints()
        {
            AmmoData data = AmmoData.GetInstance();

            foreach (string caliber in _selectedCalibers)
            {
                List<Bullet> bullets = data.GetAmmoByCaliber(caliber);

                foreach (Bullet bullet in bullets)
                {
                    Rectangle point = new Rectangle();

                    point.Fill = AmmoData.GetCaliberBrush(bullet.caliber);
                    point.RadiusX = 10; point.RadiusY = 10;
                    point.Width = 10;   point.Height = 10;
                    point.StrokeThickness = 10;

                    var penetration = bullet.penetrationPower();

                    if (penetration > 70)
                        penetration = 70;

                    double posX = (ammoCanvas.ActualWidth / maxDamage) * bullet.damage();
                    double posY = ammoCanvas.ActualHeight - (ammoCanvas.ActualHeight / 70) * penetration;
                
                    Console.WriteLine("Drawing point for bullet " + bullet.name + " at " + posX  + posY);

                    TextBlock label = new TextBlock();
                    label.Text = bullet.shortName;
                    label.Foreground = Brushes.White;
                    
                    
                    // todo REFACTOR THIS MESS!!
                    point.MouseEnter += (sender, args) =>
                    {
                        AmmoHoverTooltip tooltip = new AmmoHoverTooltip(bullet);
                        
                        Canvas.SetLeft(tooltip, posX + 10);
                        Canvas.SetTop(tooltip, posY - 5);
                        ammoCanvas.Children.Add(tooltip);
                    };

                    label.MouseEnter += (sender, args) =>
                    {
                        AmmoHoverTooltip tooltip = new AmmoHoverTooltip(bullet);
                        
                        Canvas.SetLeft(tooltip, posX + 10);
                        Canvas.SetTop(tooltip, posY - 5);
                        ammoCanvas.Children.Add(tooltip);
                    };
                    
                    point.MouseLeave += (sender, args) =>
                    {
                        ResetCanvas();
                    };
                    
                    label.MouseLeave += (sender, args) =>
                    {
                        ResetCanvas();
                    };
                
                
                    Canvas.SetLeft(point, posX - 5);
                    Canvas.SetTop(point, posY - 5);
                
                    Canvas.SetLeft(label, posX);
                    Canvas.SetTop(label, posY - 25);

                    ammoCanvas.Children.Add(point);
                    ammoCanvas.Children.Add(label);

                }
            }
            

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
            DrawGridDamageLines();
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

        private void DrawGridDamageLines(double maxDamage = 260)
        {
            double interval = (ammoCanvas.ActualWidth / maxDamage) * 20;

            for (double x = interval; x < ammoCanvas.ActualWidth; x += interval)
            {
                double currentDamageValue = ((x / interval) * 20);
                
                Line tick = new Line
                {
                    Stroke = Brushes.DarkGray,
                    StrokeThickness = 1,
                    Opacity = 0.1
                };

                tick.X1 = x; tick.X2 = x;

                tick.Y1 = 0;
                tick.Y2 = ammoCanvas.ActualHeight + 10;

                TextBlock value = new TextBlock();
                value.Text = ((x / interval) * 20).ToString();
                value.Foreground = Brushes.White;

                Canvas.SetTop(value, (ammoCanvas.ActualHeight + 20));
                Canvas.SetLeft(value, x - 10);

                ammoCanvas.Children.Add(tick);
                ammoCanvas.Children.Add(value);
            }
            
        }
    }
}