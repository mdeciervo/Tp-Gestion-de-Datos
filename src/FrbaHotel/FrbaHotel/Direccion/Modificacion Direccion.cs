using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.Direccion
{
    public partial class Modificacion_Direccion : Form
    {
        SqlDataReader resultado;
        decimal idDir;
        public Modificacion_Direccion(decimal direccion)
        {
            InitializeComponent();
            string consulta = "select nombre from GESTION_DE_GATOS.Pais";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBox1.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
            idDir = direccion;
            consulta = "select D.calle,D.numero,D.piso,D.depto,D.ciudad,P.nombre from GESTION_DE_GATOS.Direccion D,GESTION_DE_GATOS.Pais P where D.pais = P.idPais and D.idDir = " + direccion.ToString();
            resultado = Home.BD.comando(consulta);
            if (resultado.Read())
            {
                textBox1.Text = resultado.GetString(0);
                textBox2.Text = resultado.GetDecimal(1).ToString();
                if(string.IsNullOrEmpty(resultado.GetValue(2).ToString()))
                {
                }
                else
                {
                    textBox3.Text= resultado.GetDecimal(2).ToString();
                }
                if (string.IsNullOrEmpty(resultado.GetValue(3).ToString()))
                {
                }
                else
                {
                    textBox4.Text = resultado.GetString(3);
                }
                if (string.IsNullOrEmpty(resultado.GetValue(3).ToString()))
                {
                }
                else
                {
                    textBox5.Text = resultado.GetString(4);
                }
                
                comboBox1.Text = resultado.GetString(5);
            }
            resultado.Close();
            textBox1.Focus();
        }

        private void Modificacion_Direccion_Load(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
            textBox5.Text = string.Empty;
            comboBox1.ResetText();
            textBox1.Focus();
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }


        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }


        }
        private int checkearCamposVacios()
        {
            int c = 0;
            string mensaje = "";
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                c = 1;
                mensaje = mensaje + "El campo calle es obligatorio\n";
            }
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                c = 1;
                mensaje = mensaje + "El campo numero es obligatorio\n";
            }
            if (string.IsNullOrEmpty(textBox5.Text))
            {
                c = 1;
                mensaje = mensaje + "El campo ciudad es obligatorio\n";
            }
            if (string.IsNullOrEmpty(comboBox1.Text))
            {
                c = 1;
                mensaje = mensaje + "El campo pais es obligatorio\n";
            }
            if (c == 1)
            {
                MessageBox.Show(mensaje);
            }
            return c;
        }
        private int checkearValidezCampos()
        {
            int b = 0;
            string mensaje = "";
            if (Convert.ToInt32(textBox2.Text) < 0)
            {
                b = 1;
                mensaje = mensaje + "El campo numero debe contener un numero positivo\n";
            }
            if (string.IsNullOrEmpty(textBox3.Text) && !string.IsNullOrEmpty(textBox4.Text))
            {
                b = 1;
                mensaje = mensaje + "El campo piso no puede ser vacio si el campo piso esta escrito\n";
            }
            if (string.IsNullOrEmpty(textBox4.Text) && !string.IsNullOrEmpty(textBox3.Text))
            {
                b = 1;
                mensaje = mensaje + "El campo depto no puede ser vacio si el campo depto esta escrito\n";
            }
            if (b == 1)
            {
                MessageBox.Show(mensaje);
            }
            return b;
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            int a = 0;
            int b = 0;
            a = checkearCamposVacios();
            b = checkearValidezCampos();
            if (a == 0 && b == 0)
            {
                //ENTRO PORQUE SE PUEDEN EDITAR LOS DATOS

                string insert = "exec GESTION_DE_GATOS.ModificarDireccion ";
                insert = insert + idDir.ToString();
                insert = insert + ",'" + textBox1.Text + "',";
                insert = insert + textBox2.Text + ",";
                if (!string.IsNullOrEmpty(textBox3.Text))
                {
                    insert = insert + textBox3.Text + ","; ;
                }
                else
                {

                    insert = insert + "null,"; ;
                }
                if (!string.IsNullOrEmpty(textBox4.Text))
                {
                    insert = insert + "'" + textBox4.Text + "',";
                }
                else
                {
                    insert = insert + "null,";
                }
                insert = insert + "'" + textBox5.Text + "',";
                insert = insert + "'" + comboBox1.Text + "'";
                decimal resu = 0;
                resultado = Home.BD.comando(insert);
                if (resultado.Read())
                {
                    resu = resultado.GetDecimal(0);
                }
                resultado.Close();
                if (resu != 0)
                {
                    MessageBox.Show("La direccion fue guardada correctamente");
                    this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Error al guardar, la direccion ya existe");
                }

            }

        private void textBox2_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

       
        }
}

