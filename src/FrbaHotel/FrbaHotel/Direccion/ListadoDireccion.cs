using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.Direccion
{
    public partial class ListadoDireccion : Form
    {
        public static decimal direccion;
        string abm;
        string consulta;
        SqlDataReader resultado;
        SqlDataAdapter sAdapter;
        DataTable dTable;
        public ListadoDireccion(string nombreAbmPadre)
        {
            InitializeComponent();
            abm = nombreAbmPadre;
            textBox1.Focus();
            consulta = "select distinct nombre from GESTION_DE_GATOS.Pais";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBox1.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
        }

        private void listado_Load(object sender, EventArgs e)
        {
            string query = "select D.idDir id, D.calle Calle,D.numero Numero,D.piso Piso,D.depto Depto,D.ciudad Ciudad,P.nombre Pais from	GESTION_DE_GATOS.Direccion D,GESTION_DE_GATOS.Pais P where	D.pais = P.idPais";
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

        private void button1_Click(object sender, EventArgs e)
        {
            DataView dvData = new DataView(dTable);
            string query = "";
            query = query + this.filtrarAproximadamentePor("Calle", textBox1.Text);
            query = query + this.filtrarExactamentePor("Pais", comboBox1.Text);
            query = query + this.filtrarExactamentePor("Numero", textBox2.Text);
            query = query + this.filtrarExactamentePor("Piso", textBox3.Text);
            query = query + this.filtrarAproximadamentePor("Depto", textBox4.Text);
            query = query + this.filtrarAproximadamentePor("Ciudad", textBox5.Text);

            if (query.Length > 0) { query = query.Remove(query.Length - 4); }
            dvData.RowFilter = query;
            dataGridView1.DataSource = dvData;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = String.Empty;
            textBox2.Text = String.Empty;
            textBox3.Text = String.Empty;
            textBox4.Text = String.Empty;
            textBox5.Text = String.Empty;
            comboBox1.ResetText();
            this.button1_Click(null, null);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if(abm=="usuarioListado")
                {
                    paraListadoUser();
                }
                if (abm == "usuarioAlta")
                {
                    paraAltaUser();
                }
                if (abm == "usuarioModif")
                {
                    paraModifUser();
                }
                if (abm == "usuarioBaja")
                {
                    paraBajaUser();
                }
                if (abm == "clienteAlta")
                {
                    paraAltaCliente();
                }
                if (abm == "hotelAlta")
                {
                    paraAltaHotel();
                }
                if (abm == "hotelListado")
                {
                    paraListadoHotel();
                }
                if (abm == "hotelListadoM")
                {
                    paraListadoMHotel();
                }

            }
        }
        private void paraListadoMHotel()
        {
            ABM_de_Hotel.ListadoModif.direccion = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            ABM_de_Hotel.ListadoModif.ActiveForm.Show();
            this.Close();
        }
        private void paraListadoHotel()
        {
            ABM_de_Hotel.ListadoHotel.direccion = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            ABM_de_Hotel.ListadoHotel.ActiveForm.Show();
            this.Close();
        }
        private void paraAltaHotel()
        {

            ABM_de_Hotel.AltaHotel.direccion = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            ABM_de_Hotel.AltaHotel.ActiveForm.Show();
            this.Close();
        }
        private void paraBajaUser()
        {
            direccion = Convert.ToDecimal(dataGridView1.CurrentRow.Cells[1].Value);
            ABM_de_Usuario.Baja.direccion = direccion;
            ABM_de_Usuario.Baja.ActiveForm.Show();
            this.Close();
        }
        private void paraListadoUser()
        {
            direccion = Convert.ToDecimal(dataGridView1.CurrentRow.Cells[1].Value);
            ABM_de_Usuario.Listado.direccion = direccion;
            ABM_de_Usuario.Listado.ActiveForm.Show();
            this.Close();  
        }
        private void paraModifUser()
        {
            direccion = Convert.ToDecimal(dataGridView1.CurrentRow.Cells[1].Value);
            ABM_de_Usuario.ListadoModif.direccion = direccion;
            ABM_de_Usuario.ListadoModif.ActiveForm.Show();
            this.Close();
        }
        private void paraAltaUser()
        {
            direccion = Convert.ToDecimal(dataGridView1.CurrentRow.Cells[1].Value);
            ABM_de_Usuario.Alta.dir = direccion;
            ABM_de_Usuario.Alta.ActiveForm.Show();
            this.Close();
        }
        private void paraAltaCliente()
        {
            direccion = Convert.ToDecimal(dataGridView1.CurrentRow.Cells[1].Value);
            ABM_de_Cliente.Alta.dir = direccion;
            ABM_de_Cliente.Alta.ActiveForm.Show();
            this.Close();
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
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
    }
}
