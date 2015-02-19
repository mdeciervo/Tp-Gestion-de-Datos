using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FrbaHotel.Login
{
    public partial class IntermediaCliente : Form
    {
        public IntermediaCliente()
        {
            InitializeComponent();
        }

        private void cboFunCli_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Continuar_Click(object sender, EventArgs e)
        {

            if (cboFunCli.Text == "Generar o Modificar Reserva")
            {


                this.Hide();
                Generar_Modificar_Reserva.HomeReserva reserva = new Generar_Modificar_Reserva.HomeReserva();
                reserva.Show();

            }
            else
            {
                if (cboFunCli.Text == "Cancelar Reserva")
                {
                    this.Hide();
                    Cancelar_Reserva.Cancelacion canRes = new Cancelar_Reserva.Cancelacion();
                    canRes.Show();
                }
            else
            {
                MessageBox.Show("Debe seleccionar una opcion del listado");
            }
            }
        }

        private void IntermediaCliente_Load(object sender, EventArgs e)
        {

        }

        private void IntermediaCliente_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

       
    }
}
