using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace TarkovAssistantWPF
{
    /// <summary>
    /// Interaction logic for MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {

        public MapWindow()
        {
            InitializeComponent();

            pictureBox.MouseLeave += (sender, args) => panGrab = false;
            mapCanvas.IsHitTestVisible = false;

            SetMap("customs");

        }

        private void SetMap(string map)
        {

            BitmapImage bitmap = null;

            try
            {
                bitmap = new BitmapImage(new Uri($"maps/{map}.png", UriKind.Relative));
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error: file 'maps/{map}' not found!");
                return;
            }

            pictureBox.Source = bitmap;
            pictureBox.MouseUp += OnMouseUp;
        }

        double mapScale = 1;

        private bool panGrab = false;
        private Point MousePointA, MousePointB;

        private bool fullscreenEnabled = false;


        private TimeSpan clickStart = new TimeSpan(); 
        private TimeSpan clickEnd = new TimeSpan();

        private bool addingDot = true;

        #region MouseControls
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            panGrab = true;

            MousePointA = e.GetPosition(mapCanvas);

        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            MousePointB = e.GetPosition(pictureBox);

            if (panGrab)
            {
                Debug.WriteLine("Disabling dot");
                addingDot = false;

                this.Cursor = Cursors.ScrollAll;

                // how many pixels travelled since last call
                Point delta = (Point)Point.Subtract(MousePointB, MousePointA);

                // translate both the image and the canvas
                TranslateLayer(ref mapTransform, delta.X * mapScale, delta.Y * mapScale);
                TranslateLayer(ref mapCanvasTransform, delta.X * mapScale, delta.Y * mapScale);
            }

            MousePointA = e.GetPosition(pictureBox);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {


            if (sender is Canvas)
            {
                Debug.WriteLine("Sender was not image");
                e.Handled = true;
                return;
            }

            Debug.WriteLine("Mouse moving");

            if (addingDot)
            {
                var pos = e.GetPosition(mapCanvas);

                AddAreaMarker(25, pos.X, pos.Y);
            }

            if (panGrab)
            {
                // stop dragging the image, reset cursor
                panGrab = false;
                this.Cursor = Cursors.Arrow;
                addingDot = true;
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var amount = 1.2;
            var mousePoint = e.GetPosition(mapWindow);

            // remove percentage if scrolling down
            if (e.Delta < 0)
                amount = 1 / amount;

            // minimum zoom
            if (mapScale * amount <= 0.8)
                return;

            mapScale *= amount;

            ScaleLayer(ref mapTransform, amount, amount, mousePoint.X, mousePoint.Y);

            ScaleLayer(ref mapCanvasTransform, amount, amount, mousePoint.X, mousePoint.Y);
        }
        #endregion

        // Handles translating a layer, i.e. the Image or Canvas
        private void TranslateLayer(
                ref MatrixTransform transform, 
                double dX, 
                double dY
            )
        {
            var matrix = transform.Matrix;
            matrix.Translate(dX, dY);
            transform.Matrix = matrix;
        }

        // Handles scaling a layer, image or canvas
        private void ScaleLayer(
                ref MatrixTransform transform, 
                double scaleX, 
                double scaleY,
                double centerX,
                double centerY
            )
        {
            var matrix = transform.Matrix;

            matrix.ScaleAt(scaleX, scaleY, centerX, centerY);

            transform.Matrix = matrix;
        }


        private void OnKeyDown(object sender, KeyEventArgs e)
        {

            // reset
            if (e.Key == Key.R)
            {
                // remove any transformations
                mapTransform.Matrix = Matrix.Identity;
                mapCanvasTransform.Matrix = Matrix.Identity;
            }

            // fullscreen
            if (e.Key == Key.F || e.Key == Key.F11)
            {
                this.WindowState = (fullscreenEnabled) ? WindowState.Normal : WindowState.Maximized;
                this.WindowStyle = (fullscreenEnabled) ? WindowStyle.SingleBorderWindow : WindowStyle.None;

                fullscreenEnabled = !fullscreenEnabled;
            }

            // clear markers on canvas 
            if (e.Key == Key.C)
            {
                ClearCanvas();
            }
        }

        private void OnMapChange(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem) sender;

            SetMap(item.Tag as string);
        }


        #region CanvasControls

        private void AddAreaMarker(double radius, double posX, double posY)
        {
            var dotRadius = radius;

            FrameworkElement dot = new Ellipse
            {
                Fill = Brushes.DarkRed,
                Width = dotRadius,
                Height = dotRadius,
                IsHitTestVisible = false,
                Opacity = 0.5
            };

            AddMarker(ref dot, posX, posY);
        }

        private void AddMarker(ref FrameworkElement marker, double posX, double posY, bool center = true)
        {

            if (center)
            {
                posX -= marker.Width / 2;
                posY -= marker.Height / 2;
            }

            Debug.WriteLine($"Adding marker at [{posX}, {posY}]");

            mapCanvas.Children.Add(marker);


            Canvas.SetLeft(marker, posX);
            Canvas.SetTop(marker, posY);
        }

        private void ClearCanvas()
        {
            mapCanvas.Children.Clear();
        }

        #endregion

    }
}
