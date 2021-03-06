﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
using TarkovAssistantWPF.interfaces;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace TarkovAssistantWPF
{
    /// <summary>
    /// Interaction logic for MapControl.xaml
    /// </summary>
    public partial class MapControl : UserControl, IHotkeyAble
    {
        // allows a marker to be added; this is false if we are panning image (_flagPanGrab)
        private bool _flagAddMarker = true;

        private bool _flagPanGrab;

        private Map _selectedMap = Map.CUSTOMS;

        private int _subMapIndex = 0;
        private Map[] _subMaps = { };

        double mapScale = 1;


        public MapControl()
        {
            InitializeComponent();

            // we'll use the base control for translation of the image;
            // disable these hitboxes to prevent glitches when cursor leaves picturebox/canvas
            mapCanvas.IsHitTestVisible = false;
            mapImage.IsHitTestVisible = false;

            mapImage.MouseLeave += (sender, args) => _flagPanGrab = false;

            SetMap(_selectedMap);

        }


        #region MapMethods

        public void SetMap(Map map)
        {

            var mapDirectory = "/maps/";
            var fileName = map.ToString().ToLower();
            var fileExt = ".png";

            var path = mapDirectory + fileName + fileExt;

            var uri = new Uri(path, UriKind.Relative);

            Debug.WriteLine($"Loading map: {uri}, current directory: {Environment.CurrentDirectory}");

            if (map == Map.RESERVE)
            {
                _subMaps = new [] { Map.RESERVE, Map.RESERVE_TUNNELS };
            }

            _selectedMap = map;


            mapImage.Source = new BitmapImage(uri);

            mapCanvas.Width = mapImage.Width;
            mapCanvas.Height = mapImage.Height;

            ClearCanvas();
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

        private Point MousePointA, MousePointB;

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

        #region HotkeyBindings

        public void OnCycleSubMap()
        {

            if(_subMaps.Length == 0) 
                return;

            _subMapIndex++;

            // prevent outofindex
            if (_subMapIndex >= _subMaps.Length)
                _subMapIndex = 0;

            SetMap(_subMaps[_subMapIndex]);
        }

        public void OnCycleMap()
        {
            OnNextMap();
        }

        public void OnZoomIn()
        {
            var amount = 1.1;

            var mX = mapImage.ActualWidth/ 2;
            var mY = mapImage.ActualHeight / 2;

            ScaleLayer(ref mapTransform, amount, amount, mX, mY);
            ScaleLayer(ref mapCanvasTransform, amount, amount, mX, mY);
        }

        public void OnZoomOut()
        {
            var amount = 1 / 1.1;

            var mX = mapImage.ActualWidth / 2;
            var mY = mapImage.ActualHeight / 2;

            ScaleLayer(ref mapTransform, amount, amount, mX, mY);
            ScaleLayer(ref mapCanvasTransform, amount, amount, mX, mY);
        }

        public void OnNextMap()
        {
            int mapValue = (int)_selectedMap;
            int mapCount = Enum.GetValues(typeof(Map)).Length;

            if (--mapValue < 0)
            {
                mapValue = mapCount - 1;
            }

            SetMap((Map)mapValue);
        }

        public void OnPrevMap()
        {
            int mapValue = (int) _selectedMap;
            int mapCount = Enum.GetValues(typeof(Map)).Length;
            
            if (++mapValue > mapCount - 1)
            {
                mapValue = 0;
            }

            SetMap((Map) mapValue);
        }

        public void OnReset()
        {
            this.ResetTransforms();
        }

        public void OnClear()
        {
            this.ClearCanvas();
        }

        public void OnSetMap(Map mapToSet)
        {
            SetMap(mapToSet);
        }

        public void OnPan(int x, int y)
        {

            // todo add this to customizable settings
            var speed = 50;

            x *= speed;
            y *= speed;

            // translate both the image and the canvas
            TranslateLayer(ref mapTransform, x, y);
            TranslateLayer(ref mapCanvasTransform, x, y);
        }

        #endregion
    }
}
