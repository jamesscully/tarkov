using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using TarkovAssistant.Properties;
using Timer = System.Timers.Timer;

namespace TarkovAssistant
{
    public partial class TarkovMain : Form
    {
        private Image currentMap = null;
        private static Timer mouseDownLoop;
        private Canvas canvas;

        public TarkovMain()
        {
            InitializeComponent();

            // load default map
            LoadMapImage(Resources.customs);

            mouseDownLoop = new Timer
            {
                Interval = 250,
                Enabled = false,
                AutoReset = true
            };

            mouseDownLoop.Elapsed += WhileMouseDown;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            SetCanvasScales();
        }

        // Loads an Image object into the picture box - processed via RefreshDrawing()
        private void LoadMapImage(Image img)
        {
            if (picMap.Image != null)
            {
                Debug.WriteLine($"Current image size: {picMap}");

                // free image resource if we already assigned one
                picMap.Image.Dispose();
            }

            currentMap = img;

            canvas = new Canvas((Bitmap) img);


            picMap.SizeMode = PictureBoxSizeMode.Zoom;
            picMap.Dock = DockStyle.Fill;
            picMap.Image = img;


            if (picMap.Created)
            {
                SetCanvasScales();
            }
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

        // Returns the PictureBox size in pixels
        private Size GetPictureBoxSize()
        {
            Size ret = Size.Empty;

            Invoke(new Action(() =>
            {
                ret = picMap.Size;
            }));
            return ret;
        }

        // Returns our map image in pixels
        private Size GetMapPixelSize()
        {
            Size ret = Size.Empty;

            Invoke(new Action(() =>
            {
                ret = picMap.Image.Size;
            }));
            return ret;
        }


        // Mouse event handlers
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            Debug.WriteLine($"Mouse down at ({e.X}, {e.Y})");
            mouseDownLoop.Enabled = true;
            canvas.drawDot(e.Location);
            RefreshDrawing();
        }

        private void WhileMouseDown(Object source, EventArgs e)
        {
            Point point = GetCursorInPictureBox();
            Debug.WriteLine($"Mouse drag at ({point.X}, {point.Y})");
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            Debug.WriteLine($"Mouse up at ({e.X}, {e.Y})");
            mouseDownLoop.Enabled = false;
        }

        // TODO fix
        // Scales our canvas to match 1:1 with map image
        private void SetCanvasScales()
        {
            Size viewPortSize = GetPictureBoxSize();
            Size mapSize = GetMapPixelSize();

            Debug.WriteLine($"View: ({viewPortSize}) | Img: ({mapSize}) ");

            canvas.scaleX = (float) mapSize.Width  / (float) viewPortSize.Width;
            canvas.scaleY = (float) mapSize.Height / (float) viewPortSize.Height;

            Debug.WriteLine($"Set canvas scales: {canvas.scaleX}, {canvas.scaleY}");

        }

        // Draws the map with the Canvas for drawing overlayed
        private void RefreshDrawing()
        {

            // Clone our original image - funky things happen else (properties not copied?)
            Image old = picMap.Image;
            Image newMap = (Image) old.Clone();

            using (Graphics gr = Graphics.FromImage(newMap))
            {
                gr.DrawImage(old, new Point(0, 0));
                gr.DrawImage(canvas.overlay, new Point(0, 0));
            }

            // Load our new image
            LoadMapImage(newMap);
        }


    }
}
