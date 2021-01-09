using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TarkovAssistant
{
    public partial class MapViewerControl : UserControl
    {
        private Graphics canvas;
        private Image mapImage;

        private Size imageSize = Size.Empty;

        public MapViewerControl()
        {
            InitializeComponent();
        }

        public void LoadImage(Image map)
        {
            mapImage = map;
            imageSize = map.Size;

            canvas = Graphics.FromImage(mapImage);
            
            Redraw();

            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void Redraw() => pictureBox.Image = mapImage;


        private int mapX = 0, mapY = 0;

        private Pen pen = new Pen(Color.Red);

        private bool ShiftKeyHeld() => ModifierKeys.HasFlag(Keys.Shift);
        private bool CtrlKeyHeld() => ModifierKeys.HasFlag(Keys.Control);

        private void OnMouseScroll(object sender, MouseEventArgs e)
        {
            
            bool directionPositive = (e.Delta > 0);

            Debug.WriteLine("Moving Canvas, direction positive: " + directionPositive);

            int scrollStepAmount = 20;

            if(ShiftKeyHeld())
                mapX += (directionPositive) ? -(scrollStepAmount) : scrollStepAmount;
            else if (CtrlKeyHeld())
                mapY += (directionPositive) ? -(scrollStepAmount) : scrollStepAmount;

            Point newPos = new Point(mapX, mapY);

            Debug.WriteLine("Drawing image at: " + newPos);
            canvas.RotateTransform(50);

            pictureBox.Invalidate();

            // Redraw();
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {

        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Hand;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {

        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {

        }

        private void OnPaint(object sender, PaintEventArgs e)
        {

            if(mapImage != null)
                e.Graphics.DrawImage(mapImage, mapX, mapY);

            Debug.WriteLine("Picturebox is being repainted");
        }

        public void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            Debug.WriteLine("KeyPressed:" + e.KeyChar);
        }
    }
}
