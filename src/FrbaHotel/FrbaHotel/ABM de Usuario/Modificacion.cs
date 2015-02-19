using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.ABM_de_Usuario
{
    public partial class Modificacion : Form
    {
        public static decimal dir;
        private SqlDataReader resultado;
        decimal iduser;
        decimal idLoco;
        public Modificacion(decimal id,decimal ids,string username,string rol,string nombre,string apellido,string tel,string mail,string direccion,string nroDoc,string tipoDoc,string hotel,string fecha,string estado)
        {
            InitializeComponent();
            iduser = id;
            idLoco = ids;
            dir = 0;
            textBoxUser.Focus();
            string consulta;
            consulta = "select distinct descripcion from GESTION_DE_GATOS.TiposDoc";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBoxTipoDoc.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
            consulta = "select nombre from GESTION_DE_GATOS.Hotel where idHotel=" + Login.HomeLogin.hotel;
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBoxHotel.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
            consulta = "select descripcion from GESTION_DE_GATOS.Rol";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBoxRol.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
            textBoxUser.Text = username;
            comboBoxRol.Text = rol;
            textBoxNombre.Text = nombre;
            textBoxApellido.Text = apellido;
            textBoxTel.Text = tel;
            textBoxDir.Text = direccion;
            dir = Convert.ToDecimal(direccion);
            textBoxNroDoc.Text = nroDoc;
            textBoxMail.Text = mail;
            comboBoxTipoDoc.Text = tipoDoc;
            comboBoxHotel.Text = hotel;
            dateTimePicker1.Text = fecha;
            
            if (estado == "True")
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
            textBoxUser.Text = string.Empty;
            textBoxClave.Text = string.Empty;
            comboBoxRol.ResetText();
            textBoxNombre.Text = string.Empty;
            textBoxApellido.Text = string.Empty;
            textBoxTel.Text = string.Empty;
            textBoxMail.Text = string.Empty;
            textBoxDir.Text = string.Empty;
            textBoxNroDoc.Text = string.Empty;
            comboBoxTipoDoc.ResetText();
            comboBoxHotel.ResetText();
            dateTimePicker1.ResetText();
            textBoxUser.Focus();

        }
        private void textBoxNroDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBoxTel_KeyPress_1(object sender, KeyPressEventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
        {
            int a = checkearCamposVacios();
            if (a == 0)
            {
                //MODIFICAR VALORES
                string insert = "EXEC GESTION_DE_GATOS.ModificarUser ";
                insert = insert + iduser.ToString()+","+idLoco.ToString();
                insert = insert + ",'" + textBoxUser.Text + "',";
                insert = insert + "'" + Login.HomeLogin.dameHash(textBoxClave.Text) + "',";
                insert = insert + "'" + comboBoxRol.Text + "',";
                insert = insert + "'" + textBoxNombre.Text + "',";
                insert = insert + "'" + textBoxApellido.Text + "',";
                insert = insert + textBoxTel.Text + ",";
                insert = insert + "'" + textBoxMail.Text + "',";
                insert = insert + "'" + comboBoxTipoDoc.Text + "',";
                insert = insert + textBoxNroDoc.Text + ",";
                insert = insert + "'" + comboBoxHotel.Text + "',";
                DateTime fecha;

                fecha = Convert.ToDateTime(dateTimePicker1.Value);
                string lala = fecha.Date.ToString("yyyyMMdd HH:mm:ss");
                insert = insert + "'" + lala + "',";
                insert = insert + textBoxDir.Text + ",";
                if (checkBox2.Checked)
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
                    MessageBox.Show("El usuario fue guardado correctamente");
                    ListadoModif.ActiveForm.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error al guardar, el username ya existe o ese user ya trabaja para ese hotel con ese rol");
                }
            }
        }
        private int checkearCamposVacios()
        {
            int a = 0;
            string mensaje = "";
            if (string.IsNullOrEmpty(textBoxUser.Text))
            {
                a = 1;
                mensaje = mensaje + "El campo usuario es obligatorio\n";
            }
            if (string.IsNullOrEmpty(textBoxClave.Text))
            {
                a = 1;
                mensaje = mensaje + "El campo clave es obligatorio\n";
            }
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
            if (string.IsNullOrEmpty(comboBoxRol.Text))
            {
                a = 1;
                mensaje = mensaje + "El campo rol es obligatorio\n";
            }
            if (string.IsNullOrEmpty(comboBoxHotel.Text))
            {
                a = 1;
                mensaje = mensaje + "El campo hotel es obligatorio\n";
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
     }
}
