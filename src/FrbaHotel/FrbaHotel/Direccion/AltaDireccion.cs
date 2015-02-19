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
    public partial class AltaDireccion : Form
    {
        private SqlDataReader resultado;

        string abm;
        public AltaDireccion(string abmPadre)
        {
            InitializeComponent();
            abm = abmPadre;
        }

        private void AltaDireccion_Load(object sender, EventArgs e)
        {
            string consulta = "select nombre from GESTION_DE_GATOS.Pais";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBox1.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
            textBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int a=0;
            int b = 0;
            a=checkearCamposVacios();
            b=checkearValidezCampos();
            if (a ==0 && b== 0)
            {
                //ENTRO PORQUE SE PUEDEN GUARDAR LOS DATOS

                string insert = "exec GESTION_DE_GATOS.InsertarDireccion ";
                insert = insert + "'" + textBox1.Text + "',";
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
                if (resultado.Read()) {
                    resu = resultado.GetDecimal(0);
                }
                MessageBox.Show(resu.ToString());
               
                resultado.Close();
                if (resu !=0)
                {
                    MessageBox.Show("La direccion fue guardada correctamente");
                    if (abm == "usuarioAlta")
                    {
                        ABM_de_Usuario.Alta.dir = resu;
                        ABM_de_Usuario.Alta.ActiveForm.Show();
                        this.Close();
                    }
                    if (abm == "clienteAlta")
                    {
                        ABM_de_Cliente.Alta.dir = resu;
                        ABM_de_Cliente.Alta.ActiveForm.Show();
                        this.Close();
                    }
                    if (abm == "hotelAlta")
                    {
                        ABM_de_Hotel.AltaHotel.direccion = resu.ToString();
                        ABM_de_Hotel.AltaHotel.ActiveForm.Show();
                        this.Close();
                    }
                }
                else {
                    MessageBox.Show("Error al guardar, la direccion ya existe");
                }

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
                c= 1;
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
            if (Convert.ToInt32(textBox2.Text)<0)
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

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) )
            {
                e.Handled = true;
            }


        }


    }
}
