using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine;

namespace SuperAdventure
{
    public partial class SuperAdventure : Form
    {
        private Player _player;
        public SuperAdventure()
        {
            InitializeComponent();

            Location location = new Location(1, "Home", "This is your house.");

            _player = new Player(10, 10, 20, 0, 1);

            valueHitPoints.Text = _player.CurrentHitPoints.ToString();
            valueGold.Text = _player.Gold.ToString();
            valueExperience.Text = _player.ExperiencePoints.ToString();
            valueLevel.Text = _player.Level.ToString();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void buttonGoNorth_Click(object sender, EventArgs e)
        {

        }

        private void buttonGoEast_Click(object sender, EventArgs e)
        {

        }

        private void buttonGoSouth_Click(object sender, EventArgs e)
        {

        }

        private void buttonGoWest_Click(object sender, EventArgs e)
        {

        }

        private void buttonUseWeapon_Click(object sender, EventArgs e)
        {

        }

        private void buttonUsePotion_Click(object sender, EventArgs e)
        {

        }
    }
}
