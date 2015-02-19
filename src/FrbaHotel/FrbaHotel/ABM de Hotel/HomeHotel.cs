using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FrbaHotel.ABM_de_Hotel
{
    public partial class HomeHotel : Form
    {
        int cerrate = 0;
        public HomeHotel()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AltaHotel alta = new AltaHotel();
            alta.Show();
        }

        private void HomeHotel_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cerrate == 1)
            {

            }
            else
            {
                Application.Exit();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ListadoHotel listado = new ListadoHotel();
            listado.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ListadoModif modif = new ListadoModif();
            modif.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Login.HomeLogin.mainFun.Show();
            cerrate = 1;
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            BajaHotel baja = new BajaHotel();
            baja.Show();
        }

        private void HomeHotel_Load(object sender, EventArgs e)
        {

        }
    }
}
