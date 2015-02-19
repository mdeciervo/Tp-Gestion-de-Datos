using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FrbaHotel.ABM_de_Habitacion
{
    public partial class HomeHabitacion : Form
    {
        int cerrate = 0;
        public HomeHabitacion()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Listado listado = new Listado();
            listado.Show();
        }

        private void HomeHabitacion_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cerrate == 1)
            {
                
            }
            else
            {
                Application.Exit();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Login.HomeLogin.mainFun.Show();
            cerrate = 1;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Alta alta = new Alta();
            alta.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ListadoModif modif = new ListadoModif();
            modif.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Baja baja = new Baja();
            baja.Show();
        }

        private void HomeHabitacion_Load(object sender, EventArgs e)
        {

        }
    }
}
