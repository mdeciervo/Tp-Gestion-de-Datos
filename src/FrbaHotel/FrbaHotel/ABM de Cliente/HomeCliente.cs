using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FrbaHotel.ABM_de_Cliente
{
    public partial class HomeCliente : Form
    {
        int cerrate = 0;
        public HomeCliente()
        {
            InitializeComponent();
        }

        private void HomeCliente_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Alta alta = new Alta("");
            alta.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Listado listado = new Listado();
            listado.Show();
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

        private void button5_Click(object sender, EventArgs e)
        {
            Login.HomeLogin.mainFun.Show();
            cerrate = 1;
            this.Close();
        }

        private void HomeCliente_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cerrate == 1)
            {

            }
            else
            {
                Application.Exit();
            }
        }
    }
}
