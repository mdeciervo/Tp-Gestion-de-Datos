using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace FrbaHotel.ABM_de_Cliente
{
    public partial class Modificacion : Form
    {
        public static decimal dir;
        private SqlDataReader resultado;
        decimal iduser;
        public Modificacion(decimal id,string nombre,string apellido,string tipoDoc,string nroDoc,string mail,string telefono,string nacionalidad,string direccion,string fecha_nac,string Habilitado)
        {
            InitializeComponent();
            textBoxNombre.Focus();
            string consulta;
            iduser = id;
            consulta = "select distinct descripcion from GESTION_DE_GATOS.TiposDoc";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBoxTipoDoc.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
            consulta = "select distinct nombre from GESTION_DE_GATOS.Pais";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBox1.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
            textBoxNombre.Text = nombre;
            textBoxApellido.Text = apellido;
            textBoxTel.Text = telefono;
            textBoxDir.Text = direccion;
            dir = Convert.ToDecimal(direccion);
            textBoxNroDoc.Text = nroDoc;
            textBoxMail.Text = mail;
            comboBoxTipoDoc.Text = tipoDoc;
            dateTimePicker1.Text = fecha_nac;
            comboBox1.Text = nacionalidad;

            if (Habilitado == "True")
            {
                checkBox2.Checked = true;

            }
        }

        private void Modificacion_Load(object sender, EventArgs e)
        {

        }

        private void Modificacion_Activated(object sender, EventArgs e)
        {
            if (dir != 0)
            {
                textBoxDir.Text = dir.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBoxDir.Text = string.Empty;
            textBoxNombre.Text = string.Empty;
            textBoxApellido.Text = string.Empty;
            textBoxTel.Text = string.Empty;
            textBoxMail.Text = string.Empty;
            textBoxDir.Text = string.Empty;
            textBoxNroDoc.Text = string.Empty;
            comboBoxTipoDoc.ResetText();
            comboBox1.ResetText();
            dateTimePicker1.ResetText();
            textBoxNombre.Focus();
        }

        private void textBoxNroDoc_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxNroDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBoxTel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Direccion.Modificacion_Direccion modi = new Direccion.Modificacion_Direccion(dir);
            modi.Show();
        }
        private int checkearCamposVacios()
        {
            int a = 0;
            string mensaje = "";

            if (string.IsNullOrEmpty(textBoxNombre.Text))
            {
                a = 1;
                mensaje = mensaje + "El campo nombre es obligatorio\n";
            }
            if (string.IsNullOrEmpty(textBoxApellido.Text))
            {
                a = 1;
                mensaje = mensaje + "El campo apellido es obligatorio\n";
            }
            if (string.IsNullOrEmpty(comboBox1.Text))
            {
                a = 1;
                mensaje = mensaje + "El campo nacionalidad es obligatorio\n";
            }

            if (string.IsNullOrEmpty(comboBoxTipoDoc.Text))
            {
                a = 1;
                mensaje = mensaje + "El campo tipo doc es obligatorio\n";
            }
            if (string.IsNullOrEmpty(textBoxTel.Text))
            {
                a = 1;
                mensaje = mensaje + "El campo telefono es obligatorio\n";
            }
            if (string.IsNullOrEmpty(textBoxNroDoc.Text))
            {
                a = 1;
                mensaje = mensaje + "El campo NroDoc es obligatorio\n";
            }
            if (string.IsNullOrEmpty(textBoxMail.Text))
            {
                a = 1;
                mensaje = mensaje + "El campo mail es obligatorio\n";
            }
            if (string.IsNullOrEmpty(textBoxDir.Text))
            {
                a = 1;
                mensaje = mensaje + "El campo direccion es obligatorio\n";
            }

            DateTime fecha;
            fecha = Convert.ToDateTime(dateTimePicker1.Value);

            DateTime s = Home.fecha;

            int result = DateTime.Compare(fecha, s);

            if (result >= 0)
            {
                a = 1;
                mensaje = mensaje + "La fecha debe ser menor a la actual\n";
            }

            if (a == 1)
            {
                MessageBox.Show(mensaje);
            }
            return a;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int a = checkearCamposVacios();
            if (a == 0)
            {
                //INSERTAR VALORES
                string insert = "EXEC GESTION_DE_GATOS.ModificarCliente ";
                insert = insert + iduser;
                insert = insert + ",'" + textBoxNombre.Text + "',";
                insert = insert + "'" + textBoxApellido.Text + "',";
                insert = insert + textBoxTel.Text + ",";
                insert = insert + "'" + textBoxMail.Text + "',";
                insert = insert + "'" + comboBoxTipoDoc.Text + "',";
                insert = insert + textBoxNroDoc.Text + ",";

                DateTime fecha;

                fecha = Convert.ToDateTime(dateTimePicker1.Value);
                string lala = fecha.Date.ToString("yyyyMMdd HH:mm:ss");
                insert = insert + "'" + lala + "',";
                insert = insert + textBoxDir.Text + ",";
                insert = insert + "'" + comboBox1.Text + "',";
                if (iduser == 1)
                {
                    insert = insert + "1";
                }
                else
                {
                    if (checkBox2.Checked)
                    {
                        insert = insert + "1";
                    }
                    else
                    {
                        insert = insert + "0";
                    }
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
                    MessageBox.Show("El cliente fue guardado correctamente");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error al guardar, el cliente con ese nro y tipo de doc ya existe");
                }
            }
        }



    }
}
