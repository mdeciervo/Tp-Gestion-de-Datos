using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FrbaHotel.ABM_de_Usuario
{
    public partial class HomeUsuario : Form
    {
        int cerrate = 0;
        public HomeUsuario()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void LISTADO_Click(object sender, EventArgs e)
        {
            ABM_de_Usuario.Listado list = new ABM_de_Usuario.Listado();
            list.Show();
        }

        private void HomeUsuario_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cerrate == 0)
            {
                Application.Exit();
            }
        }

        private void ALTA_Click(object sender, EventArgs e)
        {
            ABM_de_Usuario.Alta alta = new ABM_de_Usuario.Alta();
            alta.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Login.HomeLogin.mainFun.Show();
            cerrate = 1;
            this.Close();
        }

        private void MODIFICACION_Click(object sender, EventArgs e)
        {
            ABM_de_Usuario.ListadoModif listaModif = new ABM_de_Usuario.ListadoModif();
            listaModif.Show();
        }

        private void BAJA_Click(object sender, EventArgs e)
        {
            ABM_de_Usuario.Baja baja = new ABM_de_Usuario.Baja();
            baja.Show();
        }

    }
}
