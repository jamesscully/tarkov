﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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
    /// Handles the Map viewer functionality; this could be embedded into another main form later on,
    /// with the introduction of item searching
    /// </summary>

    public partial class MapWindow : Window
    {
        private string selected_map = "woods";

        public MapWindow()
        {
            InitializeComponent();
            
            pictureBox.MouseLeave += (sender, args) => _flagPanGrab = false;


            // we'll use the mapWindow for translation of the image;
            // disable these hitboxes to prevent glitches when cursor leaves picturebox/canvas
            mapCanvas.IsHitTestVisible = false;
            pictureBox.IsHitTestVisible = false;

            pictureBox.MouseUp += OnMapMouseUp;

            SetMap(selected_map);

            // set search bars hint
            quickSearch.Text = Properties.Resources.str_searchhint;

            ScaleLayer(ref mapTransform, 1, 1, 0, 0);
            ScaleLayer(ref mapCanvasTransform, 1, 1, 0, 0);
        }

        double mapScale = 1;

        private Point MousePointA, MousePointB;

        private bool _fullscreen = false;
        private bool _flagAddMarker = true;
        private bool _flagPanGrab;

        #region MouseControls
        private void OnMapMouseDown(object sender, MouseButtonEventArgs e)
        {
            _flagPanGrab = true;

            MousePointA = e.GetPosition(mapWindow);
        }

        private void OnMapMouseMove(object sender, MouseEventArgs e)
        {
            MousePointB = e.GetPosition(mapWindow);

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

            MousePointA = e.GetPosition(mapWindow);
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
            var mousePoint = e.GetPosition(pictureBox);

            if (mousePoint.X < 0 || mousePoint.Y < 0)
                return;

            // remove percentage if scrolling down
            if (e.Delta < 0)
                amount = 1 / amount;

            // minimum zoom
            if (mapScale * amount <= 0.8)
                return;

            mapScale *= amount;

            if (selected_map == "woods")
            {
                ScaleLayer(ref mapTransform, amount, amount, mousePoint.X, mousePoint.Y);
                ScaleLayer(ref mapCanvasTransform, amount, amount, mousePoint.X, mousePoint.Y);
            }
            else
            {
                ScaleLayer(ref mapTransform, amount, amount, mousePoint.X, mousePoint.Y);
                ScaleLayer(ref mapCanvasTransform, amount, amount, mousePoint.X, mousePoint.Y);
            }
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


        private void OnMapKeyDown(object sender, KeyEventArgs e)
        {

            // ignore if the map doesn't have keyboard focus
            var control = (Control)sender;

            if (!control.IsKeyboardFocused)
                return;

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

            selected_map = map;


            pictureBox.Source = null;

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

            pictureBox.Source = img;

            mapCanvas.Width = pictureBox.Width;
            mapCanvas.Height = pictureBox.Height;

            ClearCanvas();
        }

        // Fired when a map button is pressed
        private void OnMapChange(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem) sender;

            SetMap(item.Tag as string);
        }

        // Return our mapsize by manually applying the transform
        // to the rendered width/height
        private Size GetMapScreenSize()
        {
            var width = pictureBox.ActualWidth;
            var height = pictureBox.ActualHeight;

            height *= (mapTransform.Matrix.M11);
            width *= (mapTransform.Matrix.M22);

            return new Size(width, height);
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

        private void ClearCanvas()
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

        #region SearchFunctions

        private void QuickSearch_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }

            if (quickSearch.Text.Length < 3 || e.Key == Key.Back)
                return;

            Debug.WriteLine($"Searching for pages: {(sender as TextBox).Text}");


            string api_url =
                $"https://escapefromtarkov.gamepedia.com/api.php?action=opensearch&format=json&formatversion=2&search={quickSearch.Text}&namespace=0&limit=3&suggest=true";


            Task.Run(() =>
            {
                WebRequest request = HttpWebRequest.Create(api_url);
                WebResponse response = request.GetResponse();

                StreamReader reader = new StreamReader(response.GetResponseStream());

                string json = reader.ReadToEndAsync().Result;

                Debug.WriteLine(json);
            });
        }

        private void QuickSearch_OnIsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            bool focused = e.NewValue.ToString() == "True";

            TextBox textbox = (TextBox) sender;

            if (focused)
            {
                textbox.Foreground = Brushes.Black;
                textbox.Text = "";
            }
            else
            {
                textbox.Foreground = Brushes.Gray;
                textbox.Text = Properties.Resources.str_searchhint;
            }
        }

        #endregion

    }
}
