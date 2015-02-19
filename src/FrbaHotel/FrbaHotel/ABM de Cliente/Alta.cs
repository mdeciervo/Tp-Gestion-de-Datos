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
    public partial class Alta : Form
    {
        public static decimal dir;
        private SqlDataReader resultado;
        string abm = "";
        public Alta(string abmPadre)
        {

            InitializeComponent();
            abm = abmPadre;
            dir = 0;
            textBoxNombre.Focus();
            string consulta;
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
            dateTimePicker1.Value = Home.fecha;
        }

        private void Alta_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Direccion.AltaDireccion altaDir = new Direccion.AltaDireccion("clienteAlta");
            altaDir.Show();
        }

        private void Alta_Activated(object sender, EventArgs e)
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

        private void textBoxTel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBoxNroDoc_KeyPress(object sender, KeyPressEventArgs e)
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

        private void button3_Click(object sender, EventArgs e)
        {
            Direccion.ListadoDireccion listaDir = new Direccion.ListadoDireccion("clienteAlta");
            listaDir.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int a = checkearCamposVacios();
            if (a == 0)
            {
                //INSERTAR VALORES
                string insert = "EXEC GESTION_DE_GATOS.InsertarCliente ";
                
                insert = insert + "'" + textBoxNombre.Text + "',";
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
                if (resu == 1)
                {
                    MessageBox.Show("El cliente fue guardado correctamente");
                    if (abm == "altaReservaCli")
                    {
                        resultado=Home.BD.comando("select idCli from GESTION_DE_GATOS.Cliente where mail = '" + textBoxMail.Text + "'");
                        if(resultado.Read())
                        {
                            Generar_Modificar_Reserva.AltaCli.cliente = resultado.GetDecimal(0).ToString();
                            Generar_Modificar_Reserva.AltaCli.ActiveForm.Show();
                        }
                        resultado.Close();
                    }
                    if (abm == "altaEstadia")
                    {
                        resultado = Home.BD.comando("select idCli,nombre,apellido from GESTION_DE_GATOS.Cliente where mail = '" + textBoxMail.Text + "'");
                        if (resultado.Read())
                        {
                            string id = resultado.GetDecimal(0).ToString();
                            string nombre = resultado.GetString(1);
                            string apellido = resultado.GetString(2);
                            DataRow row = Registrar_Estadia.ClientesEstadia.tabla.NewRow();
                            row["Id"] = id;
                            row["Nombre"] = nombre;
                            row["Apellido"] = apellido;
                            try
                            {
                                Registrar_Estadia.ClientesEstadia.tabla.Rows.Add(row);
                                Registrar_Estadia.ClientesEstadia.persDisp--;
                            }
                            catch
                            {
                                MessageBox.Show("Ese cliente ya esta agregado");
                            }

                            Registrar_Estadia.ClientesEstadia.ActiveForm.Show();
                        }
                        resultado.Close();
                    }
                    if (abm == "altaReservaUser")
                    {
                        resultado = Home.BD.comando("select idCli from GESTION_DE_GATOS.Cliente where mail = '" + textBoxMail.Text + "'");
                        if (resultado.Read())
                        {
                            Generar_Modificar_Reserva.AltaUser.cliente = resultado.GetDecimal(0).ToString();
                            Generar_Modificar_Reserva.AltaUser.ActiveForm.Show();
                        }
                        resultado.Close();
                    }
                    this.Close();
                }
                else if (resu == 2)
                {
                    MessageBox.Show("Error al guardar, el cliente con ese nro y tipo de doc ya existe");
                }
                else
                {
                    MessageBox.Show("Error al guardar, el cliente con ese mail ya existe");
                }
            }
        }
    }
}
