using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.Registrar_Estadia
{
    public partial class CheckOut : Form
    {
        string consulta;
        SqlDataReader resultado;
        SqlDataAdapter sAdapter;
        DataTable dTable;
        public CheckOut()
        {
            InitializeComponent();
            textBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private string filtrarExactamentePor(string columna, string valor)
        {
            if (!string.IsNullOrEmpty(valor))
            {
                return columna + " = '" + valor + "' AND ";
            }
            return "";
        }

        private void CheckOut_Load(object sender, EventArgs e)
        {
            string query = "select distinct CE.estadia idEstadia,CE.habitacion idHabitacion,H.numero Numero,H.piso Piso from GESTION_DE_GATOS.Estadia E, GESTION_DE_GATOS.ClienteXEstadia CE, GESTION_DE_GATOS.Habitacion H where E.salida is null and E.ingreso <= '"+Home.fecha.Date+"' and CE.estadia = E.idEstadia and CE.habitacion = H.idHabitacion and H.hotel = "+Login.HomeLogin.hotel;
            sAdapter = FrbaHotel.Home.BD.dameDataAdapter(query);
            dTable = FrbaHotel.Home.BD.dameDataTable(sAdapter);
            //BindingSource to sync DataTable and DataGridView
            BindingSource bSource = new BindingSource();
            //set the BindingSource DataSource
            bSource.DataSource = dTable;
            //set the DataGridView DataSource
            dataGridView1.DataSource = bSource;
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

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox1.Focus();
            this.button2_Click(null, null);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                //generar facturacion
                consulta = "EXEC GESTION_DE_GATOS.registrarCheckoutEstadia ";
                consulta = consulta + dataGridView1.CurrentRow.Cells[1].Value.ToString();
                consulta = consulta + ",'" + Home.fecha.Date.ToString("yyyyMMdd HH:mm:ss") + "',";
                consulta = consulta + Login.HomeLogin.idUsuario;
                resultado = Home.BD.comando(consulta);
                resultado.Read();
                if (resultado.GetDecimal(0) == 1)
                {
                    
                    resultado.Close();
                    //aca va lo de facturacion
                    Facturar_Estadia.Facturacion factu = new FrbaHotel.Facturar_Estadia.Facturacion(dataGridView1.CurrentRow.Cells[1].Value.ToString());
                    MessageBox.Show("El checkout se ha realizado correctamente. Se procede a la facturacion");
                    factu.Show();
                }
                else
                {
                    resultado.Close();
                    MessageBox.Show("El checkout no se pudo realizar correctamente");
                }
                this.Close();
            }
        }


    }
}
