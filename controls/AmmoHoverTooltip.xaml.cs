using System.Windows;
using System.Windows.Controls;
using TarkovAssistantWPF.data.models;

namespace TarkovAssistantWPF.controls
{
    public partial class AmmoHoverTooltip : UserControl
    {
        public AmmoHoverTooltip(Ammo bullet)
        {
            InitializeComponent();
            
            ammoName.Text = bullet.name;
            ammoPenetration.Text = "Penetration: " + bullet.penetrationPower();
            ammoDamage.Text = "Base Damage: " + bullet.damage();
            ammoFragmentation.Text = "Fragmentation Chance: " + (bullet.fragmentationChance() * 100).ToString("0") + "%";
            ammoArmorDamage.Text = "Armor Damage: " + bullet.armorDamage();
            ammoInitialSpeed.Text = "Muzzle Velocity: " + bullet.initialSpeed() + "m/s";

            
            
            if (bullet.recoil() > 0)
                ammoRecoil.Text = "Recoil: +" + bullet.recoil();
            else if (bullet.recoil() == 0)
                ammoRecoil.Visibility = Visibility.Collapsed;
            else
                ammoRecoil.Text = "Recoil: " + bullet.recoil();

            ammoPenetrationChance.Text = "Penetration Chance: " + (bullet.penetrationChance() * 100).ToString("0") + "%";
            ammoRicochetChance.Text = "Ricochet Chance: " + (bullet.ricochetChance() * 100).ToString("0") + "%";

        }
    }
}