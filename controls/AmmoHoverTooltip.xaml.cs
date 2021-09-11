using System.Windows.Controls;
using TarkovAssistantWPF.data;

namespace TarkovAssistantWPF.controls
{
    public partial class AmmoHoverTooltip : UserControl
    {
        public AmmoHoverTooltip(Bullet bullet)
        {
            InitializeComponent();
            
            ammoName.Text = bullet.name;
            ammoPenetration.Text = "Penetration: " + bullet.penetrationPower().ToString();
            ammoDamage.Text = "Base damage: " + bullet.damage().ToString();
        }
    }
}