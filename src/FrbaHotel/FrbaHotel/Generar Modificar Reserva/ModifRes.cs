using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.Generar_Modificar_Reserva
{
    public partial class ModifRes : Form
    {
        public ModifRes()
        {
            InitializeComponent();
            textBox1.Focus();
        }

        private void ModifRes_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Debe ingresar un numero de reserva");
                return;
            }
            SqlDataReader resultado = Home.BD.comando("Select idReserva,fecha_inicio from GESTION_DE_GATOS.Reserva where idReserva = " + textBox1.Text);
            if (resultado.Read() == true)
            {
                
                if ( (resultado.GetDecimal(0).ToString() == textBox1.Text) && (resultado.GetDateTime(1).Date >= Home.fecha.Date) )
                {
                    resultado.Close();
                    Modificacion modifica = new Modificacion(textBox1.Text);
                    modifica.Show();
                    this.Close();
                }
                else
                {
                    resultado.Close();
                    MessageBox.Show("El plazo para modificar la reserva ha vencido");
                    return;
                }
            }
            
            else
            {
                resultado.Close();
                MessageBox.Show("Numero de reserva invalido");
                return;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
