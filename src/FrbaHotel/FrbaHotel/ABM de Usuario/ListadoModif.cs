using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.ABM_de_Usuario
{
    public partial class ListadoModif : Form
    {
        public static decimal direccion = 0;
        Direccion.ListadoDireccion lista;
        private int a = 0;
        string consulta;
        SqlDataReader resultado;
        SqlDataAdapter sAdapter;
        DataTable dTable;
        public ListadoModif()
        {
            InitializeComponent();
            textBoxUser.Focus();
            consulta = "select distinct descripcion from GESTION_DE_GATOS.TiposDoc";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBoxTipoDoc.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
            consulta = "select nombre from GESTION_DE_GATOS.Hotel WHERE idHotel=" + Login.HomeLogin.hotel;
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBoxHotel.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
            consulta = "select descripcion from GESTION_DE_GATOS.Rol";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBoxRol.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
            dateTimePicker1.Value = Home.fecha;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void ListadoModif_Load(object sender, EventArgs e)
        {
            string query = "select U.userName Usuario,R.descripcion Rol,U.nombre Nombre,U.apellido Apellido,U.telefono Tel,U.mail Mail,U.nroDoc Nro_Doc,T.descripcion Tipo_Doc,D.calle Calle,D.numero Numero,D.piso Piso,D.depto Depto,D.idDir Direccion,H.nombre Hotel,U.fecha_nac Fecha_Nac,U.estado Estado from	GESTION_DE_GATOS.Usuario U,GESTION_DE_GATOS.Rol R,GESTION_DE_GATOS.UserXRolXHotel UR,GESTION_DE_GATOS.Hotel H,GESTION_DE_GATOS.Direccion D,GESTION_DE_GATOS.TiposDoc T where	U.direccion = D.idDir and	U.idUsuario = UR.usuario and UR.rol = R.idRol and UR.Hotel = H.idHotel and	U.tipoDoc = T.idTipoDoc and H.idHotel=" + Login.HomeLogin.hotel;
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

        private void button2_Click(object sender, EventArgs e)
        {
            DataView dvData = new DataView(dTable);
            string query = "";
            query = query + this.filtrarAproximadamentePor("Usuario", textBoxUser.Text);
            query = query + this.filtrarExactamentePor("Rol", comboBoxRol.SelectedText);
            query = query + this.filtrarAproximadamentePor("Nombre", textBoxNombre.Text);
            query = query + this.filtrarAproximadamentePor("Apellido", textBoxApellido.Text);
            query = query + this.filtrarExactamentePor("Tel", textBoxTel.Text);
            query = query + this.filtrarAproximadamentePor("Mail", textBoxMail.Text);
            if (a == 1)
            {
                query = query + this.filtrarExactamentePor("Direccion", textBoxDir.Text);
            }
            query = query + this.filtrarExactamentePor("Nro_Doc", textBoxNroDoc.Text);
            query = query + this.filtrarExactamentePor("Tipo_Doc", comboBoxTipoDoc.Text);
            query = query + this.filtrarExactamentePor("Hotel", comboBoxHotel.Text);
            if (checkBox1.Checked)
            {
                DateTime fecha;
                fecha = Convert.ToDateTime(dateTimePicker1.Value);
                query = query + this.filtrarExactamentePor("Fecha_Nac", fecha.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (checkBox2.Checked)
            {
                query = query + "Estado = 1 and";
            }
            else
            {
                query = query + "Estado = 0 and";
            }

            if (query.Length > 0) { query = query.Remove(query.Length - 4); }
            dvData.RowFilter = query;
            dataGridView1.DataSource = dvData;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            a = 1;
            lista = new Direccion.ListadoDireccion("usuarioModif");
            lista.Show();
        }

        private void textBoxDir_TextChanged(object sender, EventArgs e)
        {
        }

        private void ListadoModif_Shown(object sender, EventArgs e)
        {
            if (direccion != 0)
            {
                textBoxDir.Text = direccion.ToString();
            }
        }

        private void ListadoModif_Activated(object sender, EventArgs e)
        {
            if (direccion != 0)
            {
                textBoxDir.Text = direccion.ToString();
            }
            string query = "select U.userName Usuario,R.descripcion Rol,U.nombre Nombre,U.apellido Apellido,U.telefono Tel,U.mail Mail,U.nroDoc Nro_Doc,T.descripcion Tipo_Doc,D.calle Calle,D.numero Numero,D.piso Piso,D.depto Depto,D.idDir Direccion,H.nombre Hotel,U.fecha_nac Fecha_Nac,U.estado Estado from	GESTION_DE_GATOS.Usuario U,GESTION_DE_GATOS.Rol R,GESTION_DE_GATOS.UserXRolXHotel UR,GESTION_DE_GATOS.Hotel H,GESTION_DE_GATOS.Direccion D,GESTION_DE_GATOS.TiposDoc T where	U.direccion = D.idDir and	U.idUsuario = UR.usuario and UR.rol = R.idRol and UR.Hotel = H.idHotel and	U.tipoDoc = T.idTipoDoc and H.idHotel=" + Login.HomeLogin.hotel;
            sAdapter = FrbaHotel.Home.BD.dameDataAdapter(query);
            dTable = FrbaHotel.Home.BD.dameDataTable(sAdapter);
            //BindingSource to sync DataTable and DataGridView
            BindingSource bSource = new BindingSource();
            //set the BindingSource DataSource
            bSource.DataSource = dTable;
            //set the DataGridView DataSource
            dataGridView1.DataSource = bSource;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            textBoxDir.Text = string.Empty;
            textBoxUser.Text = string.Empty;
            comboBoxRol.ResetText();
            textBoxNombre.Text = string.Empty;
            textBoxApellido.Text = string.Empty;
            textBoxTel.Text = string.Empty;
            textBoxMail.Text = string.Empty;
            textBoxDir.Text = string.Empty;
            textBoxNroDoc.Text = string.Empty;
            comboBoxTipoDoc.ResetText();
            comboBoxHotel.ResetText();
            dateTimePicker1.ResetText();
            textBoxUser.Focus();
            this.button2_Click(null, null);
        }

        private void ListadoModif_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void textBoxNroDoc_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxNroDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void textBoxTel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void textBoxTel_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                decimal id=0;
                decimal idUXR = 0;
                string username = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                consulta = "select idUsuario descripcion from GESTION_DE_GATOS.Usuario where userName = '"+username+"'";
                resultado = Home.BD.comando(consulta);
                if (resultado.Read() == true)
                {
                    id = resultado.GetDecimal(0);
                }
                resultado.Close();
                string hotel = dataGridView1.CurrentRow.Cells[14].Value.ToString();
                string rol = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                consulta = "select UR.idUserXRol from GESTION_DE_GATOS.UserXRolXHotel UR, GESTION_DE_GATOS.Hotel H, GESTION_DE_GATOS.Rol R where UR.usuario = " + id + "and R.descripcion = '" + rol + "' and R.idRol = UR.rol and H.nombre = '"+hotel+"' and H.idHotel = UR.hotel";
                resultado = Home.BD.comando(consulta);
                if (resultado.Read() == true)
                {
                    idUXR = resultado.GetDecimal(0);
                }
                resultado.Close();
                
                string nombre = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                string apellido = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                string tel = dataGridView1.CurrentRow.Cells[5].Value.ToString();
                string mail = dataGridView1.CurrentRow.Cells[6].Value.ToString();
                string direccion = dataGridView1.CurrentRow.Cells[13].Value.ToString();
                string nroDoc = dataGridView1.CurrentRow.Cells[7].Value.ToString();
                string tipoDoc = dataGridView1.CurrentRow.Cells[8].Value.ToString();
                string fecha = dataGridView1.CurrentRow.Cells[15].Value.ToString();
                string estado = dataGridView1.CurrentRow.Cells[16].Value.ToString();
                Modificacion modif = new Modificacion(id,idUXR,username,rol,nombre,apellido,tel,mail,direccion,nroDoc,tipoDoc,hotel,fecha,estado);
                modif.Show();
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            a = 1;
            lista = new Direccion.ListadoDireccion("usuarioModif");
            lista.Show();
        }

        
     }
}