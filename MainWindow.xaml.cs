using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Image mapImage = new Image();

        public MainWindow()
        {
            InitializeComponent();

            pictureBox.MouseLeave += (sender, args) => panGrab = false;

            SetMap();
        }


        private bool panGrab = false;

        private Point MousePointA, MousePointB;

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            panGrab = true;

            MousePointA = e.GetPosition(relativeTo: pictureBox);
        }


        Thickness imageMargin = new Thickness();

        private void SetMap()
        {
            var bitmap = new BitmapImage(new Uri("maps/customs.png", UriKind.Relative));

            mapImage.Source = bitmap;
            mapImage.MouseUp += OnMouseUp;

            pictureBox.Children.Add(mapImage);
        }

        Point currentPoint = new Point();
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            MousePointB = e.GetPosition(pictureBox);


            if (panGrab)
            {
                // Debug.WriteLine($"A = [{MousePointA}] B = [{MousePointB}]");

                var obj = Point.Subtract(MousePointB, MousePointA);

                // Debug.WriteLine($"Obj: {obj}");

                imageMargin.Left += obj.X;
                imageMargin.Top += obj.Y;

                mapImage.Margin = imageMargin;
            }

            MousePointA = e.GetPosition(pictureBox);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            panGrab = false;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {

        }

        double mapScale = 1;

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {

                var prevScale = mapScale;
                var prevWidth = mapImage.Width;
                var prevHeight = mapImage.Height;

                

                Debug.WriteLine(e.GetPosition(mapImage));

                if (e.Delta > 0)
                    mapScale += 0.2;
                else
                    mapScale -= 0.2;

                if (mapScale <= 0.1)
                    mapScale = 0.1;

                var scaleDiff = prevScale - mapScale;

                var transform = new TransformGroup();

                var scale = new ScaleTransform(mapScale, mapScale);


                // transform.Children.Add(translate);

                transform.Children.Add(scale);

                mapImage.RenderTransform = transform;
            }
        }
    }
}
