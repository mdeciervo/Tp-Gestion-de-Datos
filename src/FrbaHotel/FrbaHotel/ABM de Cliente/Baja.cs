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
    
    public partial class Baja : Form
    {
        string consulta;
        SqlDataReader resultado;
        SqlDataAdapter sAdapter;
        DataTable dTable;
        public Baja()
        {
            InitializeComponent();
            consulta = "select distinct descripcion from GESTION_DE_GATOS.TiposDoc";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBox1.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
            textBox1.Focus();
        }

        private void Baja_Load(object sender, EventArgs e)
        {
            string query = "select C.nombre Nombre,C.apellido Apellido,T.descripcion TipoDoc,C.nroDoc NroDoc,C.mail Mail,C.telefono Telefono,P.nombre Nacionalidad,C.direccion Direccion,C.fecha_nac Fecha_Nac,C.habilitado Habilitado from GESTION_DE_GATOS.Cliente C,GESTION_DE_GATOS.Pais P,GESTION_DE_GATOS.TiposDoc T where	C.tipoDoc = T.idTipoDoc and C.nacionalidad = P.idPais";
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
            query = query + this.filtrarAproximadamentePor("Nombre", textBox1.Text);
            query = query + this.filtrarExactamentePor("TipoDoc", comboBox1.Text);
            query = query + this.filtrarAproximadamentePor("Apellido", textBox2.Text);
            query = query + this.filtrarExactamentePor("NroDoc", textBox4.Text);
            query = query + this.filtrarAproximadamentePor("Mail", textBox3.Text);
            if (query.Length > 0) { query = query.Remove(query.Length - 4); }
            dvData.RowFilter = query;
            dataGridView1.DataSource = dvData;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            comboBox1.ResetText();
            textBox1.Focus();
            this.button2_Click(null, null);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                Direccion.mostraDir mostrar = new Direccion.mostraDir(dataGridView1.CurrentRow.Cells[9].Value.ToString());
                mostrar.Show();
            }
            if (e.ColumnIndex == 1)
            {
                if (dataGridView1.CurrentRow.Cells[11].Value.ToString() == "False")
                {
                    MessageBox.Show("El cliente ya esta dado de baja");
                    return;
                }
                decimal id=0; 
                
                string mail = dataGridView1.CurrentRow.Cells[6].Value.ToString();
                consulta = "select idCli from GESTION_DE_GATOS.Cliente where mail = '"+mail+"'";
                resultado = Home.BD.comando(consulta);
                if (resultado.Read() == true)
                {
                    id = resultado.GetDecimal(0);
                }
                resultado.Close();
                if (MessageBox.Show("Estas seguro que desea inhabilitar al cliente?", "AVISO", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    consulta = "update GESTION_DE_GATOS.Cliente set habilitado=0 where idCli = " + id;

                    resultado = Home.BD.comando(consulta);
                    if (resultado.Read() == true)
                    {
                    }
                    resultado.Close();
                }
                dTable = FrbaHotel.Home.BD.dameDataTable(sAdapter);
                //BindingSource to sync DataTable and DataGridView
                BindingSource bSource = new BindingSource();
                //set the BindingSource DataSource
                bSource.DataSource = dTable;
                //set the DataGridView DataSource
                dataGridView1.DataSource = bSource;
                
            }

        
        }

        private void Baja_Activated(object sender, EventArgs e)
        {
    
        }
    }
}
