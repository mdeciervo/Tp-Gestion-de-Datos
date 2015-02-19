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
    public partial class ListadoHotel : Form
    {
        public static string direccion = "";
        BindingSource bSource;
        Direccion.ListadoDireccion lista;
       
      
        SqlDataAdapter sAdapter;
        DataTable dTable;
        public ListadoHotel()
        {
            InitializeComponent();
            dateTimePicker1.Value = Home.fecha;
            textBox1.Focus();
        }

        private void ListadoHotel_Load(object sender, EventArgs e)
        {
            string query = "select nombre Nombre,mail Mail,telefono Tel,direccion Direccion,cant_estrellas Estrellas,recarga_estrella RecargaEstrella,fecha_creacion FechaCreacion from	GESTION_DE_GATOS.Hotel";
            sAdapter = FrbaHotel.Home.BD.dameDataAdapter(query);
            dTable = FrbaHotel.Home.BD.dameDataTable(sAdapter);
            //BindingSource to sync DataTable and DataGridView
            bSource = new BindingSource();
            //set the BindingSource DataSource
            bSource.DataSource = dTable;
            //set the DataGridView DataSource
            dataGridView1.DataSource = bSource;
        }
        private string filtrarExactamentePor(string columna, string valor)
        {
            if (!string.IsNullOrEmpty(valor))
            {
                return columna + " = '" + valor + "' AND ";
            }
            return "";
        }

        private string filtrarAproximadamentePor(string columna, string valor)
        {
            if (!string.IsNullOrEmpty(valor))
            {
                return columna + " LIKE '%" + valor + "%' AND ";
            }
            return "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataView dvData = new DataView(dTable);
            string query = "";
            
            query = query + this.filtrarAproximadamentePor("Nombre", textBox1.Text);
            query = query + this.filtrarAproximadamentePor("Mail", textBox2.Text);
            query = query + this.filtrarExactamentePor("Tel", textBox3.Text);
            query = query + this.filtrarExactamentePor("Direccion", textBox4.Text);
            query = query + this.filtrarExactamentePor("Estrellas", textBox5.Text);
            query = query + this.filtrarExactamentePor("RecargaEstrella", textBox6.Text);
            if (checkBox1.Checked)
            {
                query = query + this.filtrarExactamentePor("FechaCreacion", dateTimePicker1.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (query.Length > 0) { query = query.Remove(query.Length - 4); }
            MessageBox.Show(query);
            dvData.RowFilter = query;
            dataGridView1.DataSource = dvData;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            lista = new Direccion.ListadoDireccion("hotelListado");
            lista.Show();
        }

        private void ListadoHotel_Activated(object sender, EventArgs e)
        {
            if (direccion != "")
            {
                textBox4.Text = direccion.ToString();
            }
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

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            checkBox1.Checked = false;
            dateTimePicker1.Value = Home.fecha;
            direccion = "";
            dataGridView1.DataSource = bSource;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                Direccion.mostraDir mostrar = new Direccion.mostraDir(dataGridView1.CurrentRow.Cells[4].Value.ToString());
                mostrar.Show();
            }
        }

    }
}
