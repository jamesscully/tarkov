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
using Timer = System.Timers.Timer;

namespace TarkovAssistant
{
    public partial class MapViewerControl : UserControl
    {
        private Graphics canvas;
        private Image mapImage;

        private Size imageSize = Size.Empty;

        private Timer mouseDownLoop;

        public MapViewerControl()
        {
            InitializeComponent();

            mouseDownLoop = new Timer
            {
                Interval = 25,
                Enabled = false,
                AutoReset = true
            };
        }

        public void LoadImage(Image map)
        {


            mapImage = map;
            imageSize = map.Size;

            if (canvas != null)
                canvas.Clear(Color.White);

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

            int scrollStepAmount = 20;

            if (!directionPositive)
                scrollStepAmount = -(scrollStepAmount);

            if (ShiftKeyHeld())
                PanImage(scrollStepAmount, 0);
            else if (!CtrlKeyHeld())
                PanImage(0, scrollStepAmount);

            pictureBox.Invalidate();
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {

        }

        Point MousePointA = Point.Empty;
        Point MousePointB = Point.Empty;

        private bool panGrab = false;

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Hand;
            panGrab = true;
            mouseDownLoop.Enabled = true;
            mouseDownLoop.Elapsed += (o, args) =>
            {

            };

            MousePointA = Cursor.Position;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (panGrab)
            {
                // calculate our position (and deltas) from absolute values,
                // rather than relative (in-form)
                MousePointB = Cursor.Position;

                int deltaX = MousePointB.X - MousePointA.X;
                int deltaY = MousePointB.Y - MousePointA.Y;

                PanImage(deltaX, deltaY);

                MousePointA = Cursor.Position;
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            panGrab = false;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {

            e.Graphics.Clear(Color.White);

            if(mapImage != null)
                e.Graphics.DrawImage(mapImage, mapX, mapY);
        }

        public void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            Debug.WriteLine("KeyPressed:" + e.KeyChar);
        }


        public void PanImage(int dX, int dY)
        {
            mapX += dX;
            mapY += dY;

            Redraw();
        }
    }
}
