using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.ABM_de_Habitacion
{
    public partial class Alta : Form
    {
        private SqlDataReader resultado;
        public Alta()
        {
            InitializeComponent();
            string consulta = "select nombre from GESTION_DE_GATOS.Hotel where idHotel = " + Login.HomeLogin.hotel;
            resultado = Home.BD.comando(consulta);
            if (resultado.Read())
            {
                textBox6.Text = resultado.GetString(0);
            }
            resultado.Close();
            consulta = "select distinct descripcion from GESTION_DE_GATOS.TipoHabitacion";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBox1.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
            textBox1.Focus();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Alta_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            checkBox1.Checked = false;
            comboBox1.ResetText();
            textBox1.Focus();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int a = checkearCamposVacios();
            if (a == 0)
            {
                //INSERTAR VALORES
                string insert = "EXEC GESTION_DE_GATOS.InsertarHabitacion ";
                insert = insert + textBox1.Text + ",";
                insert = insert + textBox2.Text + ",";
                insert = insert + "'" + textBox3.Text + "',";
                insert = insert + "'" + comboBox1.Text + "',";
                insert = insert + Login.HomeLogin.hotel + ",";
                if (!string.IsNullOrEmpty(textBox4.Text))
                {
                    insert = insert + "'" + textBox4.Text + "',";
                }
                else
                {
                    insert = insert + "NULL,";
                }
                
                if (checkBox1.Checked)
                {
                    insert = insert + "1";
                }
                else
                {
                    insert = insert + "0";
                }

                decimal resu = 0;
                resultado = Home.BD.comando(insert);
                if (resultado.Read())
                {
                    resu = resultado.GetDecimal(0);
                }
                resultado.Close();
                if (resu != 0)
                {
                    MessageBox.Show("La habitacion fue guardada correctamente");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error al guardar, la habitacion ya existe con ese numero,piso,tipo y hotel");
                }
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private int checkearCamposVacios()
        {
            int a = 0;
            string mensaje = "";
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                a = 1;
                mensaje = mensaje + "El campo numero es obligatorio\n";
            }
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                a = 1;
                mensaje = mensaje + "El campo piso es obligatorio\n";
            }
            if (string.IsNullOrEmpty(textBox3.Text))
            {
                a = 1;
                mensaje = mensaje + "El campo ubicacion es obligatorio\n";
            }
            if (string.IsNullOrEmpty(textBox6.Text))
            {
                a = 1;
                mensaje = mensaje + "El campo hotel es obligatorio\n";
            }
            if (string.IsNullOrEmpty(comboBox1.Text))
            {
                a = 1;
                mensaje = mensaje + "El campo tipo es obligatorio\n";
            }
                

            if (a == 1)
            {
                MessageBox.Show(mensaje);
            }
            return a;
        }

    }
}
