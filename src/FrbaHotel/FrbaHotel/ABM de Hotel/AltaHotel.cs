using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.ABM_de_Hotel
{
    public partial class AltaHotel : Form
    {
        public static string direccion = "";//numero de direccion
        DataTable tabla;
        BindingSource bSource2;
        private SqlDataReader resultado;
        SqlDataAdapter sAdapter;
        DataTable dTable;
        string consulta;
        public AltaHotel()
        {
            InitializeComponent();
            dateTimePicker1.Value = Home.fecha;
            tabla = new DataTable();
            tabla.Columns.Add("Id");
            DataColumn column = tabla.Columns["Id"];
            column.Unique = true;
            tabla.Columns.Add("Descripcion");
            bSource2 = new BindingSource();
            bSource2.DataSource = tabla;
            //set the DataGridView DataSource
            dataGridView2.DataSource = bSource2;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                string id = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                string descripcion = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                
                int item = dataGridView1.CurrentRow.Index;

                DataRow row = tabla.NewRow();
                row["Id"] = id;
                row["Descripcion"] = descripcion;
                try
                {
                    tabla.Rows.Add(row);
                    dataGridView1.Rows.RemoveAt(item);
                    dataGridView2.DataSource = bSource2;
                 
                }
                catch
                {
                    MessageBox.Show("Ese regimen ya esta agregado");
                }


            }
        }

        private void AltaHotel_Load(object sender, EventArgs e)
        {
            direccion = "";
            string query = "select distinct codigo Id,descripcion Descripcion from GESTION_DE_GATOS.Regimen";

            sAdapter = FrbaHotel.Home.BD.dameDataAdapter(query);
            dTable = FrbaHotel.Home.BD.dameDataTable(sAdapter);
            //BindingSource to sync DataTable and DataGridView
            BindingSource bSource = new BindingSource();
            //set the BindingSource DataSource
            bSource.DataSource = dTable;
            //set the DataGridView DataSource
            dataGridView1.DataSource = bSource;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Direccion.ListadoDireccion listaDir = new Direccion.ListadoDireccion("hotelAlta");
            listaDir.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Direccion.AltaDireccion altaDir = new Direccion.AltaDireccion("hotelAlta");
            altaDir.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            dateTimePicker1.Value = Home.fecha;
            tabla.Clear();
            AltaHotel_Load(null, null);
        }

        private void AltaHotel_Activated(object sender, EventArgs e)
        {
            if (direccion != "")
            {
                textBox4.Text = direccion;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Debe ingresar un nombre");
                return;
            }
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Debe ingresar un mail");
                return;
            }
            if (string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("Debe ingresar un telefono");
                return;
            }
            if (string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Debe ingresar una direccion");
                return;
            }
            if (tabla.Rows.Count == 0)
            {
                MessageBox.Show("Debe ingresar por lo menos un regimen");
                return;
            }
            if (string.IsNullOrEmpty(textBox5.Text))
            {
                MessageBox.Show("Debe ingresar la cantidad de estrellas");
                return;
            }
            if (string.IsNullOrEmpty(textBox6.Text))
            {
                MessageBox.Show("Debe ingresar la recarga x estrella");
                return;
            }

            string insert = "EXEC GESTION_DE_GATOS.InsertarHotel ";
            insert = insert + "'" + textBox1.Text + "',";
            insert = insert + "'" + textBox2.Text + "',";
            insert = insert  + textBox3.Text + ",";
            insert = insert + textBox4.Text + ",";
            insert = insert + textBox5.Text + ",";
            insert = insert + textBox6.Text + ",";
            insert = insert + "'" + dateTimePicker1.Value.Date.ToString("yyyyMMdd HH:mm:ss") + "'";
           
            resultado = Home.BD.comando(insert);
            resultado.Read();
            decimal idhotel = 0;
            if (resultado.GetDecimal(0) != 0)
            {
                idhotel = resultado.GetDecimal(0);
            }
            else
            {
                resultado.Close();
                MessageBox.Show("Ese hotel ya esta ingresado");
                return;
            }
            resultado.Close();
            consulta = "exec GESTION_DE_GATOS.InsertarUserXRolXHotel ";
            consulta = consulta + Login.HomeLogin.idUsuario + ",";
            consulta = consulta + idhotel.ToString() + ",";
            consulta = consulta +"'"+ Login.HomeLogin.rol + "'";
            
            resultado = Home.BD.comando(consulta);
            resultado.Read();

            if (resultado.GetDecimal(0) != 0)
            {
               
            }
            else
            {
                resultado.Close();
                MessageBox.Show("Ese hotel ya esta ingresado para ese user con ese rol");
                return;
            }
            resultado.Close();

            foreach (DataRow fila in tabla.Rows)
            {
                resultado = Home.BD.comando("EXEC GESTION_DE_GATOS.InsertarRegimenXHotel " + fila["Id"].ToString() + "," + idhotel);
                if (resultado.Read())
                {
                    if (resultado.GetDecimal(0) == 0)
                    {
                        MessageBox.Show("Error. El hotel ya tenia ese regimen agregado");
                    }
                }
                else
                {
                    MessageBox.Show("Error. El hotel ya tenia ese regimen agregado");
                }
                resultado.Close();
            }
            MessageBox.Show("El proceso de carga del hotel finalizo correctamente");
            this.Close();



        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
