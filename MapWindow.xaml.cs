using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

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

            pictureBox.MouseLeave += (sender, args) => _flagPanGrab = false;
            mapCanvas.IsHitTestVisible = false;

            pictureBox.MouseUp += OnMouseUp;

            SetMap("customs");


            ScaleLayer(ref mapTransform, 1, 1, 0, 0);
            ScaleLayer(ref mapCanvasTransform, 1, 1, 0, 0);
        }

        double mapScale = 5;

        private Point MousePointA, MousePointB;

        private bool _fullscreen = false;

        private bool _flagAddMarker = true;

        private bool _flagPanGrab;

        #region MouseControls
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _flagPanGrab = true;

            MousePointA = e.GetPosition(mapCanvas);

        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            MousePointB = e.GetPosition(pictureBox);

            if (_flagPanGrab)
            {
                _flagAddMarker = false;

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
            if (_flagAddMarker)
            {
                var pos = e.GetPosition(mapCanvas);
                AddAreaMarker(25, pos.X, pos.Y);
            }

            if (_flagPanGrab)
            {
                // stop dragging the image, reset cursor
                _flagPanGrab = false;
                this.Cursor = Cursors.Arrow;
                _flagAddMarker = true;
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var amount = 1.1;
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

            Debug.WriteLine($"Canvas Width      : {mapCanvas.Width}");
            Debug.WriteLine($"Canvas ActualWidth: {mapCanvas.ActualWidth}");


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
                this.WindowState = (_fullscreen) ? WindowState.Normal : WindowState.Maximized;
                this.WindowStyle = (_fullscreen) ? WindowStyle.SingleBorderWindow : WindowStyle.None;

                _fullscreen = !_fullscreen;
            }

            // clear markers on canvas 
            if (e.Key == Key.C)
            {
                ClearCanvas();
            }
        }

        #region MapMethods


        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private void SetMap(string map)
        {

            Debug.WriteLine($"Drawing map {map}, {new Uri($"maps/{map}.png", UriKind.Relative)}");

            

            var resolvedImage = (Bitmap) Properties.Resources.ResourceManager.GetObject(map);

            if (resolvedImage == null)
            {
                Debug.WriteLine($"Error: resource {map}.png could not be found!");
                return;
            }

            pictureBox.Source = null;

            IntPtr hBitmap = resolvedImage.GetHbitmap();
            ImageSource img;

            try
            {
                img = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                );
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            pictureBox.Source = img;

            ClearCanvas();
        }

        private void OnMapChange(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem) sender;

            SetMap(item.Tag as string);
        }

        #endregion

        private BitmapImage GenerateBitmap(string mapname)
        {
            BitmapImage bitmap = new BitmapImage();

            bitmap.BeginInit();

            bitmap.CacheOption = BitmapCacheOption.OnDemand;
            bitmap.UriSource = new Uri($"maps/{mapname}.png", UriKind.Relative);

            // bitmap.DecodePixelWidth = (int) pictureBox.ActualWidth;

            bitmap.EndInit();

            return bitmap;
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
