using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TarkovAssistant.Properties;

namespace TarkovAssistant
{
    public partial class TarkovMain : Form
    {

        private PictureBox picMap = new PictureBox();

        public TarkovMain()
        {
            InitializeComponent();
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            if (panel1 != null)
            {

                picMap.Location = new Point(0, 50);
                picMap.SizeMode = PictureBoxSizeMode.AutoSize;

                panel1.AutoScroll = true;
                panel1.Controls.Add(picMap);
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
        }
    }
}
