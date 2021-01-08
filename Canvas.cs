using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TarkovAssistant
{
    class Canvas
    {
        private Point startPoint = Point.Empty;
        private Point endPoint   = Point.Empty;

        private static int strokeWidth = 50;
        private Color strokeColour = Color.Red;

        public float scaleX = 1;
        public float scaleY = 1;

        public PictureBox view;

        // bitmap to write any drawing to
        public Bitmap overlay;

        public Canvas(Bitmap map)
        {
            OnChangeMap(map);
        }

        public void OnChangeMap(Bitmap map)
        {

            overlay?.Dispose();

            overlay = (Bitmap) map.Clone();

            Debug.WriteLine($"Canvas: Creating layer with dimensions: {overlay.Size}");
        }

        public void drawDot(Point point)
        {

            int x = (int) (point.X * scaleX);
            int y = (int) (point.Y * scaleY);

            Debug.WriteLine($"Canvas: Drawing dot at ({x}, {y}), screen: {point}");

            using (Graphics g = Graphics.FromImage(overlay))
            {
                // create a circle where we clicked, scaled to image
                g.FillEllipse(
                    Brushes.Magenta,
                    // center the dot to the cursor
                    x - strokeWidth / 2,
                    y - strokeWidth / 2, 
                    strokeWidth, 
                    strokeWidth
                );
            }
        }
    }
}
