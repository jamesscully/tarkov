using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
using Size = System.Windows.Size;

namespace TarkovAssistantWPF
{
    /// <summary>
    /// Interaction logic for MapControl.xaml
    /// </summary>
    public partial class MapControl : UserControl
    {
        private bool _flagAddMarker = true;
        private bool _flagPanGrab;
        private Map selected_map;

        double mapScale = 1;
        private Point MousePointA, MousePointB;

        public MapControl()
        {
            InitializeComponent();

            // we'll use the base control for translation of the image;
            // disable these hitboxes to prevent glitches when cursor leaves picturebox/canvas
            mapCanvas.IsHitTestVisible = false;
            mapImage.IsHitTestVisible = false;

            mapImage.MouseLeave += (sender, args) => _flagPanGrab = false;

            SetMap(selected_map);

        }


        #region MapMethods

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public void SetMap(Map map)
        {

            Debug.WriteLine($"Drawing map {map}, {new Uri($"maps/{map}.png", UriKind.Relative)}");

            var resolvedImage = (Bitmap)Properties.Resources.ResourceManager.GetObject(map.ToString().ToLower());

            if (resolvedImage == null)
            {
                Debug.WriteLine($"Error: resource {map}.png could not be found!");
                return;
            }

            selected_map = map;


            mapImage.Source = null;

            // get raw bitmap
            IntPtr hBitmap = resolvedImage.GetHbitmap();
            ImageSource img;

            try
            {
                // attempt to convert bitmap -> image source
                img = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                );
            }
            finally
            {
                // remove any resources attached to bitmap
                DeleteObject(hBitmap);
            }

            mapImage.Source = img;

            mapCanvas.Width = mapImage.Width;
            mapCanvas.Height = mapImage.Height;

            ClearCanvas();
        }


        // Return our mapsize by manually applying the transform
        // to the rendered width/height
        private Size GetMapScreenSize()
        {
            var width = mapImage.ActualWidth;
            var height = mapImage.ActualHeight;

            height *= (mapTransform.Matrix.M11);
            width *= (mapTransform.Matrix.M22);

            return new Size(width, height);
        }

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

        public void ResetTransforms()
        {
            mapCanvasTransform.Matrix = Matrix.Identity;
            mapTransform.Matrix = Matrix.Identity;
        }

        #endregion





        #region CanvasControls

        // used to create a default 'area' marker
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

        // Adds a given marker (control) to the canvas
        private void AddMarker(ref FrameworkElement marker, double posX, double posY, bool center = true)
        {

            if (center)
            {
                posX -= marker.Width / 2;
                posY -= marker.Height / 2;
            }

            mapCanvas.Children.Add(marker);

            double relX = mapCanvas.ActualWidth / posX;
            double relY = mapCanvas.ActualHeight / posY;

            Canvas.SetLeft(marker, posX);
            Canvas.SetTop(marker, posY);
        }

        public void ClearCanvas()
        {
            mapCanvas.Children.Clear();
        }

        // Fires on canvas resize; this will calculate each markers new position
        private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // width/height scale changes
            var dX = e.NewSize.Width / e.PreviousSize.Width;
            var dY = e.NewSize.Height / e.PreviousSize.Height;

            foreach (UIElement mark in mapCanvas.Children)
            {
                var left = Canvas.GetLeft(mark);
                var top = Canvas.GetTop(mark);

                Canvas.SetLeft(mark, left * dX);
                Canvas.SetTop(mark, top * dY);
            }
        }

        #endregion



        #region Events

        #region MouseEvents

        private void OnMapMouseDown(object sender, MouseButtonEventArgs e)
        {
            _flagPanGrab = true;

            MousePointA = e.GetPosition(this);
        }

        private void OnMapMouseMove(object sender, MouseEventArgs e)
        {
            MousePointB = e.GetPosition(this);

            if (_flagPanGrab)
            {
                _flagAddMarker = false;

                this.Cursor = Cursors.ScrollAll;

                // how many pixels traveled since last call
                Point delta = (Point)Point.Subtract(MousePointB, MousePointA);

                double panSpeed = 0.8;

                // translate both the image and the canvas
                TranslateLayer(ref mapTransform, delta.X * panSpeed, delta.Y * panSpeed);
                TranslateLayer(ref mapCanvasTransform, delta.X * panSpeed, delta.Y * panSpeed);
            }

            MousePointA = e.GetPosition(this);
        }

        private void OnMapMouseUp(object sender, MouseButtonEventArgs e)
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

        private void OnMapMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var amount = 1.1;
            var mousePoint = e.GetPosition(mapImage);

            if (mousePoint.X < 0 || mousePoint.Y < 0)
                return;

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

        #region KeyboardEvents

        #endregion

        #endregion
    }
}
