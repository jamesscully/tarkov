﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TarkovAssistant.Properties;
using Timer = System.Timers.Timer;

namespace TarkovAssistant
{
    public partial class TarkovMain : Form
    {

        private PictureBox picMap = new PictureBox();

        private Image originalImage = null;
        private Size preScrollSize = Size.Empty;

        private static Timer mouseDownLoop;

        public TarkovMain()
        {
            InitializeComponent();

            mouseDownLoop = new Timer
            {
                Interval = 10,
                Enabled = false,
                AutoReset = true
            };

            mouseDownLoop.Elapsed += WhileMouseDown;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            if (panel1 != null)
            {

                picMap.Location = new Point(0, 50);
                picMap.SizeMode = PictureBoxSizeMode.AutoSize;

                panel1.AutoScroll = true;
                panel1.Controls.Add(picMap);

                picMap.MouseDown += this.OnMouseDown;
                picMap.MouseUp += this.OnMouseUp;
            }
        }


        private void OnMapContainerLoad(object sender, ControlEventArgs e)
        {
            // load default map
            LoadMapImage(Resources.customs);
        }

        // Loads an Image object into the picture box - processed via RefreshDrawing()
        private void LoadMapImage(Image img)
        {
            if (picMap.Image != null)
            {
                // free image resource if we already assigned one
                picMap.Image.Dispose();
            }

            originalImage = (Image) img.Clone();

            picMap.Image = img;


        }

        // Fires when a map button is pressed
        private void MapButtonClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            string mapName = button.Text.ToLower();

            Bitmap bmp = null;

            try
            {
                bmp = (Bitmap) Resources.ResourceManager.GetObject(mapName);
            }
            catch (Exception)
            {
                // load customs map if the map name is not found
                Debug.WriteLine("Error retrieving resource (name): " + mapName);
                bmp = Resources.customs;
            }

            LoadMapImage(bmp);
        }


        private void WriteDebugPicmap()
        {
            Debug.WriteLine("Panel Size: " + panel1.Size);
            Debug.WriteLine("Form Size: " + this.Size);
            Debug.WriteLine("PicMap Size:" + picMap.Size);
        }

        private bool fullscreen = false;
        private void ToggleFullscreen()
        {
            fullscreen = !fullscreen;

            if (fullscreen)
            {
                Debug.WriteLine("Entered Fullscreen");

                // update the og. picturebox size before we can scroll or do anything!
                preScrollSize = picMap.Size;

                // Maximizes the form and image (to scale), hides UI
                this.TopMost = false;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.picMap.SizeMode = PictureBoxSizeMode.Zoom;

                // set form background to dark when in fullscreen
                BackColor = Color.Black;

                // readjust PictureBox size to its container
                picMap.Size = panel1.Size;

                // we have no margin to account for now (with the UI header)
                picMap.Location = Point.Empty;

                UIControlsHeader.Hide();
                panel1.AutoScroll = false;

            }
            else
            {
                Debug.WriteLine("Exited Fullscreen");

                // inverse of above 

                this.TopMost = true;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.WindowState = FormWindowState.Normal;
                picMap.SizeMode = PictureBoxSizeMode.AutoSize;
                panel1.AutoScroll = true;

                picMap.Location = new Point(0, 50);

                ResetBackColor();

                UIControlsHeader.Show();
            }

            WriteDebugPicmap();
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            Debug.WriteLine("Pressed: " + e.KeyChar);

            if (e.KeyChar == 'F' || e.KeyChar == 'f')
                ToggleFullscreen();

            if (e.KeyChar == 'R' || e.KeyChar == 'r')
            {
                if (fullscreen)
                {
                    // set our picturebox to original size, apply normal fullscreen settings
                    picMap.Size = preScrollSize;
                    picMap.SizeMode = PictureBoxSizeMode.Zoom;
                    picMap.Size = panel1.Size;
                    picMap.Location = Point.Empty;
                }
            }
        }

        private void OnMouseWheelScroll(object sender, MouseEventArgs e)
        {
            bool zoomIn = e.Delta > 0;

            int zoomAmount = 50;

            if (zoomIn)
            {
                picMap.Width += zoomAmount;
                picMap.Height += zoomAmount;
            }
            else
            {
                picMap.Width -= zoomAmount;
                picMap.Height -= zoomAmount;
            }
        }


        private Point mouseDragStart = Point.Empty;
        private Point mouseDragEnd = Point.Empty;

        // Mouse event handlers
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            mouseDragStart = e.Location;
            mouseDownLoop.Enabled = true;

        }
        // Returns our cursor relative to the picture/map control
        private Point GetCursorInPictureBox()
        {
            Point ret = Point.Empty;
            Invoke(new Action(() =>
            {
                ret = picMap.PointToClient(Cursor.Position);
            }));
            return ret;
        }

        private void WhileMouseDown(Object source, EventArgs e)
        {
            mouseDragEnd = GetCursorInPictureBox();

            int deltaX = (int) (mouseDragEnd.X - mouseDragStart.X);
            int deltaY = (int) (mouseDragStart.Y - mouseDragEnd.Y);

            Debug.WriteLine("Mousedown, delta: " + deltaX + " " + deltaY);

            picMap.Invoke((MethodInvoker) delegate
            {
                int processedX = picMap.Location.X + deltaX;
                int processedY = picMap.Location.Y - deltaY;

                picMap.Location = new Point(processedX, processedY);

            });

            mouseDragStart = GetCursorInPictureBox();
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            mouseDownLoop.Enabled = false;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {

        }
    }
}
