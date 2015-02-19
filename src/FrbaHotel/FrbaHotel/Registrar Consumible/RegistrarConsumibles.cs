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
    public partial class RegistrarConsumibles : Form
    {
        int cerrate = 0;
       
        SqlDataAdapter sAdapter;
        DataTable dTable;
        public RegistrarConsumibles()
        {
            InitializeComponent();
            textBox1.Focus();
        }
        private string filtrarExactamentePor(string columna, string valor)
        {
            if (!string.IsNullOrEmpty(valor))
            {
                return columna + " = '" + valor + "' AND ";
            }
            return "";
        }


        private void RegistrarConsumibles_Load(object sender, EventArgs e)
        {
            string query = "select distinct CE.estadia idEstadia,CE.habitacion idHabitacion,H.numero Numero,H.piso Piso from GESTION_DE_GATOS.Estadia E, GESTION_DE_GATOS.ClienteXEstadia CE, GESTION_DE_GATOS.Habitacion H where E.salida is null and E.ingreso <= '" + Home.fecha.Date + "' and CE.estadia = E.idEstadia and CE.habitacion = H.idHabitacion and H.hotel = " + Login.HomeLogin.hotel;
            sAdapter = FrbaHotel.Home.BD.dameDataAdapter(query);
            dTable = FrbaHotel.Home.BD.dameDataTable(sAdapter);
            //BindingSource to sync DataTable and DataGridView
            BindingSource bSource = new BindingSource();
            //set the BindingSource DataSource
            bSource.DataSource = dTable;
            //set the DataGridView DataSource
            dataGridView1.DataSource = bSource;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
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
            textBox1.Focus();
            this.button2_Click(null, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataView dvData = new DataView(dTable);
            string query = "";
            query = query + this.filtrarExactamentePor("idEstadia", textBox1.Text);
            query = query + this.filtrarExactamentePor("idHabitacion", textBox2.Text);
            query = query + this.filtrarExactamentePor("Numero", textBox3.Text);
            query = query + this.filtrarExactamentePor("Piso", textBox4.Text);
            if (query.Length > 0) { query = query.Remove(query.Length - 4); }
            dvData.RowFilter = query;
            dataGridView1.DataSource = dvData;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                string idEstadia = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                string idHabitacion = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                string numero = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                string piso = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                Consumibles consu = new Consumibles(idEstadia, idHabitacion, numero, piso);
                consu.Show();
                cerrate = 1;
                this.Close();
            }
        }

        private void RegistrarConsumibles_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cerrate == 0)
            {
                Login.HomeLogin.mainFun.Show();
            }
            else
            {
            }
            
        }
    }
}
