using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using TarkovAssistantWPF.data.models;

namespace TarkovAssistantWPF.data
{
    public class AmmoData : BaseDataClass<Ammo>
    {
        private static AmmoData _instance;
        
        private HashSet<string> Calibers = new HashSet<string>();

        private AmmoData()
        {
            DATA_LOCATION = Constants.DATA_LOCATION_AMMO;
            
            Load( b =>
            {
                Ammo ammo = (Ammo) b;
                Calibers.Add(ammo.caliber);
                return true;
            });
        }
        
        public static AmmoData GetInstance()
        {
            if (_instance == null)
                _instance = new AmmoData();

            return _instance;
        }

        public List<string> GetAllCalibers()
        {
            return Calibers.ToList();
        }

        public List<Ammo> GetAmmoByCaliber(string caliber)
        {
            List<Ammo> output = new List<Ammo>();
            
            foreach (var keyValuePair in Data)
            {
                Ammo ammo = keyValuePair.Value as Ammo;
                if (ammo.caliber == caliber)
                {
                    output.Add(ammo);
                }
            }

            return output;
        }

        // Return proper naming for caliber
        public static string NormalizeCaliberName(string caliber)
        {
            switch (caliber)
            {
                case "Caliber9x18PM": return "9x18PM";
                case "Caliber762x51": return "7.62x51mm";
                case "Caliber762x25TT": return "7.62x25TT";
                case "Caliber9x19PARA": return "9x19mm";
                case "Caliber556x45NATO": return "5.56x45mm NATO";
                case "Caliber545x39": return "5.45x39mm";
                case "Caliber762x54R": return "7.62x54R";
                case "Caliber46x30": return "46x30mm";
                case "Caliber366TKM": return ".366 TKM";
                case "Caliber20g": return "20 gauge";
                case "Caliber762x39": return "7.62x39mm";
                case "Caliber127x108": return "12.7x108mm";
                case "Caliber30x29": return "30x29mm";
                case "Caliber9x21": return "9x21mm";
                case "Caliber40mmRU": return "40mmRU";
                case "Caliber9x39": return "9x39mm";
                case "Caliber127x55": return "12.7x55mm";
                case "Caliber12g": return "12 gauge";
                case "Caliber57x28": return "5.7x28mm";
                case "Caliber1143x23ACP": return ".45 ACP";
                case "Caliber23x75": return "23x75mm";
                case "Caliber40x46": return "40x46mm";
                case "Caliber762x35": return "7.62x35mm";
                case "Caliber86x70": return ".338 Lapua";
                default:
                    return caliber;
            }
        }
        
        public static Brush GetCaliberBrush(string caliber)
        {
            switch (caliber)
            {
                case "Caliber9x18PM": return Brushes.Aqua;
                case "Caliber762x51": return Brushes.Beige;
                case "Caliber762x25TT": return Brushes.Brown;
                case "Caliber9x19PARA": return Brushes.Fuchsia;
                case "Caliber556x45NATO": return Brushes.Gold;
                case "Caliber545x39": return Brushes.Chartreuse;
                case "Caliber762x54R": return Brushes.Plum;
                case "Caliber46x30": return Brushes.Red;
                case "Caliber366TKM": return Brushes.OrangeRed;
                case "Caliber20g": return Brushes.OliveDrab;
                case "Caliber762x39": return Brushes.MediumVioletRed;
                case "Caliber127x108": return Brushes.MediumSpringGreen;
                case "Caliber30x29": return Brushes.DarkSlateBlue;
                case "Caliber9x21": return Brushes.Chocolate;
                case "Caliber40mmRU": return Brushes.Azure;
                case "Caliber9x39": return Brushes.Indigo;
                case "Caliber127x55": return Brushes.DarkSalmon;
                case "Caliber12g": return Brushes.Gainsboro;
                case "Caliber57x28": return Brushes.Tomato;
                case "Caliber1143x23ACP": return Brushes.Coral;
                case "Caliber23x75": return Brushes.Silver;
                case "Caliber40x46": return Brushes.IndianRed;
                case "Caliber762x35": return Brushes.DarkKhaki;
                case "Caliber86x70": return Brushes.Crimson;
                default:
                    return Brushes.DarkMagenta;
            }
        }
    }
}