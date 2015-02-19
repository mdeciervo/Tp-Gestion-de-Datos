using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.ABM_de_Rol
{
    public partial class Listado : Form
    {
     
   
        SqlDataAdapter sAdapter;
        DataTable dTable;
        public Listado()
        {
            InitializeComponent();
            textBox1.Focus();
            comboBox2.Items.Add("Activo");
            comboBox2.Items.Add("Inactivo");
        }

        private void Listado_Load(object sender, EventArgs e)
        {
            string query = "select idRol ID,descripcion Descripcion, estado Estado from GESTION_DE_GATOS.Rol";
            
            sAdapter = FrbaHotel.Home.BD.dameDataAdapter(query);
            dTable = FrbaHotel.Home.BD.dameDataTable(sAdapter);
            //BindingSource to sync DataTable and DataGridView
            BindingSource bSource = new BindingSource();
            //set the BindingSource DataSource
            bSource.DataSource = dTable;
            //set the DataGridView DataSource
            dataGridView1.DataSource = bSource;
        }

        
        
        private string filtrarBool(string columna, string valor)
        {
            if (!string.IsNullOrEmpty(valor)&&valor=="Activo")
            {
                return columna + " = " + 1.ToString() +" AND ";
            } 
            if (!string.IsNullOrEmpty(valor) && valor == "Inactivo")
            {
                return columna + " = " + 0.ToString() + " AND ";
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
            textBox1.Clear();
            comboBox2.ResetText();

            textBox1.Focus();
            dataGridView1.ClearSelection();
            dataGridView2.DataSource = null;
            button1_Click(null, null);

        }
        private void button1_Click(object sender, EventArgs e)
        {

            DataView dvData = new DataView(dTable);
            string query = "";
            query = query + this.filtrarAproximadamentePor("Descripcion", textBox1.Text);
            query = query + this.filtrarBool("Estado", comboBox2.Text);
            if (query.Length > 0) { query = query.Remove(query.Length - 4); }
            dvData.RowFilter = query;
            dataGridView1.DataSource = dvData;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                string query = "select F.idFuncionalidad ID,F.denominacion Denominacion from GESTION_DE_GATOS.Funcionalidad F, GESTION_DE_GATOS.FuncXRol FR where F.idFuncionalidad = FR.funcionalidad and FR.rol = " + dataGridView1.CurrentRow.Cells[1].Value.ToString(); ;
                SqlDataAdapter sAdapter2;
                DataTable dTable2;
                sAdapter2 = FrbaHotel.Home.BD.dameDataAdapter(query);
                dTable2 = FrbaHotel.Home.BD.dameDataTable(sAdapter2);
                //BindingSource to sync DataTable and DataGridView
                BindingSource bSource = new BindingSource();
                //set the BindingSource DataSource
                bSource.DataSource = dTable2;
                //set the DataGridView DataSource
                dataGridView2.DataSource = bSource;
            }
        }


        
    }
}
