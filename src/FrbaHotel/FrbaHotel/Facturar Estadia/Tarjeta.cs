using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.Facturar_Estadia
{
    public partial class Tarjeta : Form
    {
        string factura;
        public Tarjeta(string fact)
        {

            InitializeComponent();
            factura = fact;
            textBox1.Focus();
            comboBox1.Items.Add("Visa");
            comboBox1.Items.Add("Master Card");
            comboBox1.Items.Add("American Express");
            comboBox2.Items.Add("Banco Rio");
            comboBox2.Items.Add("Banco Frances");
            comboBox2.Items.Add("Banco Provincia");
            comboBox2.Items.Add("Banco Patagonia");
            comboBox2.Items.Add("Banco ICBC");

        }

        private void Tarjeta_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Debe ingresar un numero de tarjeta");
                return;
            }
            if (string.IsNullOrEmpty(comboBox1.Text))
            {
                MessageBox.Show("Debe ingresar una entidad de tarjeta");
                return;
            }
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Debe ingresar al titular de la tarjeta");
                return;
            }
            if (string.IsNullOrEmpty(comboBox2.Text))
            {
                string consulta = "EXEC GESTION_DE_GATOS.registrarTarjeta '" + comboBox1.Text + "'," + textBox1.Text + ",null,'" + textBox2.Text + "'";
                SqlDataReader resultado = Home.BD.comando(consulta);
                resultado.Read();
                if (resultado.GetDecimal(0) != 0)
                {
                    resultado.Close();
                    MessageBox.Show("La tarjeta fue agregada con exito");
                    this.Close();
                }
                else
                {
                    resultado.Close();
                    MessageBox.Show("La tarjeta ya estaba ingresada");
                    this.Close();
                }
            }
            else
            {
                string consulta = "EXEC GESTION_DE_GATOS.registrarTarjeta "+factura+",'" + comboBox1.Text + "'," + textBox1.Text + ",'" +comboBox2.Text+"','" + textBox2.Text + "'";
                SqlDataReader resultado = Home.BD.comando(consulta);
                resultado.Read();
                if (resultado.GetDecimal(0) != 0)
                {
                    resultado.Close();
                    MessageBox.Show("La tarjeta fue agregada con exito");
                    this.Close();
                }
                else
                {
                    resultado.Close();
                    MessageBox.Show("La tarjeta ya fue ingresada");
                }
            }


            
        }
    }
}
