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
using TarkovAssistant.Properties;

namespace TarkovAssistant
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void LoadMapImage(Image img)
        {
            
            if (picMap.Image != null)
            {
                Debug.WriteLine("Image was not null, disposing picMap");
                picMap.Image.Dispose();
            }


            picMap.SizeMode = PictureBoxSizeMode.StretchImage;
            picMap.Image = img;
        }

        private void MapButtonClick(object sender, EventArgs e)
        {
            Button button = (Button) sender;

            Bitmap bmp = null;

            string mapName = button.Text.ToLower();


            switch (mapName)
            {
                case "shoreline": bmp = new Bitmap(TarkovAssistant.Properties.Resources.shoreline); break;
                case "interchange": bmp = new Bitmap(TarkovAssistant.Properties.Resources.interchange); break;
            }
            //
            string mapDirectory = "./maps/";
            string mapFileExt = ".png";
            //

            // Debug.WriteLine("Finding image: " + mapDirectory + mapName + mapFileExt);
            //
            // LoadMapImage(Image.FromFile(mapDirectory + mapName + mapFileExt));

            LoadMapImage(bmp);
        }
    }
}
