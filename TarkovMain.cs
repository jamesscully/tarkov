using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using TarkovAssistant.Properties;

namespace TarkovAssistant
{
    public partial class TarkovMain : Form
    {
        private Bitmap currentMap = null;

        public TarkovMain()
        {
            InitializeComponent();

            // load default map
            LoadMapImage(Resources.customs);

        }

        private void LoadMapImage(Image img)
        {
            
            if (picMap.Image != null)
            {
                // free image resource if we already assigned one
                picMap.Image.Dispose();
            }

            picMap.SizeMode = PictureBoxSizeMode.Zoom;
            picMap.Image = img;

            currentMap = new Bitmap(img);
        }

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

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            Debug.WriteLine($"Mouse down at ({e.X}, {e.Y})");
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            Debug.WriteLine($"Mouse up at ({e.X}, {e.Y})");
        }
    }
}
