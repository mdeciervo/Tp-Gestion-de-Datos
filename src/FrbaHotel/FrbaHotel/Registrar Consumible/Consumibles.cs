using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.Registrar_Consumible
{
    public partial class Consumibles : Form
    {
      
        DataTable tablaConsu;
        BindingSource bSource2;
        private SqlDataReader resultado;
        SqlDataAdapter sAdapter;
        DataTable tablaReg;
       
        public Consumibles(string estadia,string habitacion,string numero, string piso)
        {
            InitializeComponent();
            textBox1.Text = estadia;
            textBox2.Text = habitacion;
            textBox3.Text = numero;
            textBox4.Text = piso;
            tablaReg = new DataTable();
            tablaReg.Columns.Add("Id");
            tablaReg.Columns.Add("Precio");
            tablaReg.Columns.Add("Descripcion");
            bSource2 = new BindingSource();
            bSource2.DataSource = tablaReg;
            //set the DataGridView DataSource
            dataGridView2.DataSource = bSource2;
        }

        private void Consumibles_Load(object sender, EventArgs e)
        {
            string query = "select idConsumible,precio,descripcion from GESTION_DE_GATOS.Consumibles";
            sAdapter = FrbaHotel.Home.BD.dameDataAdapter(query);
            tablaConsu = FrbaHotel.Home.BD.dameDataTable(sAdapter);
            //BindingSource to sync DataTable and DataGridView
            BindingSource bSource = new BindingSource();
            //set the BindingSource DataSource
            bSource.DataSource = tablaConsu;
            //set the DataGridView DataSource
            dataGridView1.DataSource = bSource;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tablaReg.Clear();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                string id = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                string precio = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                string descripcion = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                DataRow row = tablaReg.NewRow();
                row["Id"] = id;
                row["Precio"] = precio;
                row["Descripcion"] = descripcion;
                tablaReg.Rows.Add(row);
                dataGridView2.DataSource = bSource2;
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                int item = dataGridView2.CurrentRow.Index;
                tablaReg.Rows.RemoveAt(item);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (tablaReg.Rows.Count == 0)
            {
                MessageBox.Show("Debe seleccionar por lo menos 1 Consumible");
                return;
            }
            foreach (DataRow fila in tablaReg.Rows)
            {
                resultado = Home.BD.comando("EXEC GESTION_DE_GATOS.RegistrarConsXEstadiaXHab "+ textBox1.Text +","+ fila["Id"].ToString() + "," +textBox2.Text);
                if (resultado.Read())
                {
                    if (resultado.GetDecimal(0) == 0)
                    {
                        MessageBox.Show("Error. El consumible ya estaba agregado");
                    }
                }
                else
                {
                    MessageBox.Show("Error. El consumible ya estaba agregado");
                }
                resultado.Close();
            }
            MessageBox.Show("El proceso de carga de consumibles finalizo correctamente");
            this.Close();

        }

        private void Consumibles_FormClosing(object sender, FormClosingEventArgs e)
        {
            Login.HomeLogin.mainFun.Show();
        }

    }
}
