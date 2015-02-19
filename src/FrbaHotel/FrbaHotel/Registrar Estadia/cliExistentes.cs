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
    public partial class cliExistentes : Form
    {
        public static decimal direccion = 0;
        string consulta;
        SqlDataReader resultado;
        SqlDataAdapter sAdapter;
        DataTable dTable;
        
        public cliExistentes()
        {
            InitializeComponent();
            consulta = "select distinct descripcion from GESTION_DE_GATOS.TiposDoc";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBox1.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
            textBox3.Focus();
        }

        private void cliExistentes_Load(object sender, EventArgs e)
        {
            string query = "select C.idCli Id ,C.nombre Nombre,C.apellido Apellido,T.descripcion TipoDoc,C.nroDoc NroDoc,C.mail Mail from GESTION_DE_GATOS.Cliente C,GESTION_DE_GATOS.TiposDoc T where T.idTipoDoc=C.tipoDoc";
            sAdapter = FrbaHotel.Home.BD.dameDataAdapter(query);
            dTable = FrbaHotel.Home.BD.dameDataTable(sAdapter);
            //BindingSource to sync DataTable and DataGridView
            BindingSource bSource = new BindingSource();
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

            query = query + this.filtrarExactamentePor("TipoDoc", comboBox1.Text);

            query = query + this.filtrarExactamentePor("NroDoc", textBox4.Text);
            query = query + this.filtrarAproximadamentePor("Mail", textBox3.Text);
            if (query.Length > 0) { query = query.Remove(query.Length - 4); }
            dvData.RowFilter = query;
            dataGridView1.DataSource = dvData;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox3.Clear();
            textBox4.Clear();
            comboBox1.ResetText();
            textBox3.Focus();
            this.button2_Click(null, null);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                string id = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                string nombre = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                string apellido = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                DataRow row = ClientesEstadia.tabla.NewRow();
                row["Id"] = id;
                row["Nombre"] = nombre;
                row["Apellido"] = apellido;
                try
                {
                    ClientesEstadia.tabla.Rows.Add(row);
                    ClientesEstadia.persDisp--;
                }
                catch
                {
                    MessageBox.Show("Ese cliente ya esta agregado");
                }
               
               ClientesEstadia.ActiveForm.Show();
               this.Close();
            }
        }
    }
}
