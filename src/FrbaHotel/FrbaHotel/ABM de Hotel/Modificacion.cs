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
    public partial class Modificacion : Form
    {
        string idHotel;
        BindingSource bSource2;
        BindingSource bSource1;
        DataTable tabla1;
        DataTable tabla2;
        public static decimal dir;
        private SqlDataReader resultado;
        public Modificacion(string id, string nombre, string mail, string tel, string estrellas, string recarga, string direc, string fecha)
        {
            InitializeComponent();
            idHotel = id;
            textBox1.Text = nombre;
            textBox2.Text = mail;
            textBox3.Text = tel;
            textBox4.Text = direc;
            textBox5.Text = estrellas;
            textBox6.Text = recarga;
            if (!string.IsNullOrEmpty(fecha))
            {
                dateTimePicker1.Value = Convert.ToDateTime(fecha);
            }
            else
            {
                dateTimePicker1.Value = Home.fecha;
            }

            


        }

        private void Modificacion_Load(object sender, EventArgs e)
        {
            tabla2 = new DataTable();
            tabla2.Columns.Add("Id");
            DataColumn column = tabla2.Columns["Id"];
            column.Unique = true;
            tabla2.Columns.Add("Descripcion");
            bSource2 = new BindingSource();
            bSource2.DataSource = tabla2;
            //set the DataGridView DataSource
            dataGridView2.DataSource = bSource2;

            string consulta = "select R.codigo, R.descripcion from GESTION_DE_GATOS.Regimen R, GESTION_DE_GATOS.RegimenXHotel RH where RH.regimen = R.codigo and RH.hotel = " + idHotel;
            resultado = Home.BD.comando(consulta);
            while (resultado.Read())
            {
                DataRow row = tabla2.NewRow();
                row["Id"] = resultado.GetDecimal(0);
                row["Descripcion"] = resultado.GetString(1);
                tabla2.Rows.Add(row);
            }
            resultado.Close();

            tabla1 = new DataTable();
            tabla1.Columns.Add("Id");
            column = tabla2.Columns["Id"];
            column.Unique = true;
            tabla1.Columns.Add("Descripcion");
            bSource1 = new BindingSource();
            bSource1.DataSource = tabla1;
            //set the DataGridView DataSource
            dataGridView1.DataSource = bSource1;

            consulta = "select codigo,descripcion from GESTION_DE_GATOS.Regimen where codigo not in " + "(select R.codigo from GESTION_DE_GATOS.Regimen R, GESTION_DE_GATOS.RegimenXHotel RH where RH.regimen = R.codigo and RH.hotel = " + idHotel + ")";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read())
            {
                DataRow row = tabla1.NewRow();
                row["Id"] = resultado.GetDecimal(0);
                row["Descripcion"] = resultado.GetString(1);
                tabla1.Rows.Add(row);
            }
            resultado.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Direccion.Modificacion_Direccion modi = new Direccion.Modificacion_Direccion(Convert.ToDecimal(textBox4.Text));
            modi.Show();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                string id = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                string descripcion = dataGridView1.CurrentRow.Cells[2].Value.ToString();

                int item = dataGridView1.CurrentRow.Index;

                DataRow row = tabla2.NewRow();
                row["Id"] = id;
                row["Descripcion"] = descripcion;
                try
                {
                    tabla2.Rows.Add(row);
                    tabla1.Rows.RemoveAt(item);
                    dataGridView2.DataSource = bSource2;
                    dataGridView1.DataSource = bSource1;


                }
                catch
                {
                    MessageBox.Show("Ese regimen ya esta agregado");
                }


            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                string id = dataGridView2.CurrentRow.Cells[1].Value.ToString();
                string descripcion = dataGridView2.CurrentRow.Cells[2].Value.ToString();

                int item = dataGridView2.CurrentRow.Index;

                DataRow row = tabla1.NewRow();
                row["Id"] = id;
                row["Descripcion"] = descripcion;
                try
                {
                    tabla1.Rows.Add(row);
                    tabla2.Rows.RemoveAt(item);
                    dataGridView1.DataSource = bSource1;
                    dataGridView2.DataSource = bSource2;

                }
                catch
                {
                    MessageBox.Show("Ese regimen ya esta agregado");
                }


            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            
            textBox5.Clear();
            textBox6.Clear();
            dateTimePicker1.Value = Home.fecha;
            tabla2.Clear();
            tabla1.Clear();
            string query = "select distinct codigo Id,descripcion Descripcion from GESTION_DE_GATOS.Regimen";
            SqlDataAdapter sAdapter = FrbaHotel.Home.BD.dameDataAdapter(query);
            tabla1 = FrbaHotel.Home.BD.dameDataTable(sAdapter);
            //BindingSource to sync DataTable and DataGridView
            BindingSource bSource = new BindingSource();
            //set the BindingSource DataSource
            bSource.DataSource = tabla1;
            //set the DataGridView DataSource
            dataGridView1.DataSource = bSource;
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
            if (tabla2.Rows.Count == 0)
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

            string insert = "EXEC GESTION_DE_GATOS.ModificarHotel ";
            insert = insert + idHotel + ",";
            insert = insert + "'" + textBox1.Text + "',";
            insert = insert + "'" + textBox2.Text + "',";
            insert = insert + textBox3.Text + ",";
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
                MessageBox.Show("Ese hotel no se puede modificar");
                return;
            }
            resultado.Close();

            string borrado = "EXEC GESTION_DE_GATOS.BorrarRegimenesXHotel " + idHotel;
            resultado = Home.BD.comando(borrado);
            resultado.Read();
            if (resultado.GetDecimal(0)==1)
            {

            }
            else
            {
                MessageBox.Show("No se pudieron borrar los regimenes del hotel");
            }
            resultado.Close();

            foreach (DataRow fila in tabla2.Rows)
            {
                MessageBox.Show("EXEC GESTION_DE_GATOS.InsertarRegimenXHotel " + fila["Id"].ToString() + "," + idhotel);
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
            MessageBox.Show("El proceso de modificacion del hotel finalizo correctamente");
            
            this.Close();
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

    }
}
