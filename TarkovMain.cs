using System;
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
                Interval = 25,
                Enabled = false,
                AutoReset = true
            };

            mouseDownLoop.Elapsed += WhileMouseDown;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            if (panel1 != null)
            {
                picMap.Location = new Point(0, UIControlsHeader.Height);
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
            Button button = (Button) sender;

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
                ApplyFullscreenSettings();

                panel1.AutoScroll = true;


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
                    ApplyFullscreenSettings();
                }
            }
        }

        // encapsulates applying fullscreen settings; use as a 'reset' for fullscreen
        private void ApplyFullscreenSettings()
        {
            // set our picturebox to original size, apply normal fullscreen settings
            picMap.Size = preScrollSize;
            picMap.SizeMode = PictureBoxSizeMode.Zoom;
            picMap.Size = panel1.Size;
            picMap.Location = Point.Empty;
        }

        private float scrollScale = 1f;
        
        private void OnMouseWheelScroll(object sender, MouseEventArgs e)
        {
            // zoom if we hold Ctrl whilst scrolling; most zoom implementations function this way
            if (ModifierKeys.HasFlag(Keys.Control))
            {
                HandleMapZoom(e.Delta);
            }

            bool panUp = e.Delta > 0;
            int panAmount = (panUp) ? -25 : 25;

            // if we're holding shift, we move left-right, else up/down.
            if(ModifierKeys.HasFlag(Keys.Shift)) 
                PanImage(panAmount, 0);
            else
                PanImage(0, panAmount);
        }

        private void HandleMapZoom(float delta)
        {
            bool zoomIn = delta > 0;
            float zoomAmount = (zoomIn) ? 0.1f : -0.1f;
            int width = preScrollSize.Width, height = preScrollSize.Height;

            scrollScale += zoomAmount;

            picMap.Width  = (int) (width  * scrollScale);
            picMap.Height = (int) (height * scrollScale);
        }

        private Point mouseDragStart = Point.Empty;
        private Point mouseDragEnd = Point.Empty;

        // Mouse event handlers
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            mouseDragStart = GetCursorInPictureBox();
            mouseDownLoop.Enabled = true;

            Cursor.Current = Cursors.Hand;
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

        private bool IsImageOutsideBounds()
        {
            var pictureBounds = picMap.Bounds;
            var panelBounds = panel1.Bounds;

            var intersect = Rectangle.Intersect(pictureBounds, panelBounds);

            // Rectangle.Intersect will return empty if there is no intersection
            return (intersect == Rectangle.Empty);
        }

        private void PanImage(int dX, int dY)
        {
            picMap.Invoke((MethodInvoker) delegate
            {
                int processedX = picMap.Location.X + dX;
                int processedY = picMap.Location.Y - dY;
                picMap.Location = new Point(processedX, processedY);
            });

        }

        private void WhileMouseDown(Object source, EventArgs e)
        {
            mouseDragEnd = GetCursorInPictureBox();

            int deltaX = (int) (mouseDragEnd.X - mouseDragStart.X);
            int deltaY = (int) (mouseDragStart.Y - mouseDragEnd.Y);

            if (deltaX == 0 && deltaY == 0)
                return;

            PanImage(deltaX, deltaY);

            mouseDragStart = GetCursorInPictureBox();
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            mouseDownLoop.Enabled = false;

            if (IsImageOutsideBounds())
            {
                ApplyFullscreenSettings();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {

        }
    }
}
