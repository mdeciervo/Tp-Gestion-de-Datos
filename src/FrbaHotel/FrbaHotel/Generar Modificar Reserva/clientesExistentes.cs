using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace FrbaHotel.Generar_Modificar_Reserva
{
    public partial class clientesExistentes : Form
    {
        public static decimal direccion = 0;
        string consulta;
        SqlDataReader resultado;
        SqlDataAdapter sAdapter;
        DataTable dTable;
        string abm;
        public clientesExistentes(string abmp)
        {
            
            InitializeComponent();
            abm = abmp;
            consulta = "select distinct descripcion from GESTION_DE_GATOS.TiposDoc";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBox1.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
            textBox3.Focus();

        }

        private void clientesExistentes_Load(object sender, EventArgs e)
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

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

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
                if (abm == "altaReservaCli")
                {
                    Generar_Modificar_Reserva.AltaCli.cliente = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                    Generar_Modificar_Reserva.AltaCli.ActiveForm.Show();
                }
                if (abm == "altaReservaUser")
                {
                    Generar_Modificar_Reserva.AltaUser.cliente = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                    Generar_Modificar_Reserva.AltaUser.ActiveForm.Show();
                }
                this.Close();
            }
        }

    }
}
