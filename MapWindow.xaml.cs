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

            pictureBox.OpacityMask = Brushes.Black;

            SetMap("customs");
        }

        private string mapName = "customs";

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

            mapName = map;

        }

        double mapScale = 1;

        private bool panGrab = false;
        private Point MousePointA, MousePointB;

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            panGrab = true;

            MousePointA = e.GetPosition(pictureBox);

            this.Cursor = Cursors.ScrollAll;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            MousePointB = e.GetPosition(pictureBox);

            if (panGrab)
            {
                // how many pixels travelled since last call
                Point delta = (Point) Point.Subtract(MousePointB, MousePointA);

                var matrix = mapTransform.Matrix;

                // use map scale to keep constant pan speed when zoomed in
                matrix.Translate(delta.X * mapScale, delta.Y * mapScale);

                mapTransform.Matrix = matrix;
            }

            MousePointA = e.GetPosition(pictureBox);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (panGrab)
            {
                // stop dragging the image, reset cursor
                panGrab = false;
                this.Cursor = Cursors.Arrow;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.R)
            {

                Console.WriteLine("Resetting map");
                SetMap("interchange");
            }
            else
            {
                Console.WriteLine($"{mapScale}");

            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e) { }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var amount = 1.2;
            var mousePoint = e.GetPosition(mapWindow);
            var matrix = mapTransform.Matrix;

            // remove percentage if scrolling down
            if (e.Delta < 0)
                amount = 1 / amount;

            // minimum zoom
            if (mapScale * amount <= 0.8)
                return;

            mapScale *= amount;

            matrix.ScaleAt(amount, amount, mousePoint.X, mousePoint.Y);

            mapTransform.Matrix = matrix;
        }

        private void OnMapChange(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem) sender;

            SetMap(item.Tag as string);
        }

    }
}
