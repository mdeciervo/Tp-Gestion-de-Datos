using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.Login
{
    public partial class IntermediaUsuarioConRoles : Form
    {
        private int cerrate = 0;
        private SqlDataReader resultado;
        public IntermediaUsuarioConRoles()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string consulta="exec GESTION_DE_GATOS.BuscarHotelDeUser '"+comboBox1.SelectedItem+"' , "+Login.HomeLogin.idUsuario.ToString()+";";
            resultado = Home.BD.comando(consulta);
            comboBox2.Items.Clear();
            while (resultado.Read() == true)
            {
                comboBox2.Items.Add(resultado.GetDecimal(0));
            }
            resultado.Close();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void IntermediaUsuarioConRoles_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cerrate == 1)
            {

            }
            else
            {
                Application.Exit();
            }
        }

        private void IntermediaUsuarioConRoles_Load(object sender, EventArgs e)
        {
            string consulta = "select distinct R.descripcion from GESTION_DE_GATOS.UserXRolXHotel U,GESTION_DE_GATOS.Rol R where U.rol=R.idRol and R.estado = 1 and U.usuario = " + Login.HomeLogin.idUsuario.ToString();
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBox1.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
            consulta = "select distinct H.idHotel Id, H.nombre Nombre from GESTION_DE_GATOS.UserXRolXHotel U,GESTION_DE_GATOS.Hotel H where U.hotel=H.idHotel and U.usuario = " + Login.HomeLogin.idUsuario.ToString();
            DataTable result = Home.BD.consulta(consulta);
            dataGridView1.DataSource = result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(comboBox1.SelectedIndex) != -1 && Convert.ToInt32(comboBox2.SelectedIndex) != -1)
            {
                Login.HomeLogin.rol = comboBox1.SelectedItem.ToString();
                Login.HomeLogin.hotel = comboBox2.SelectedItem.ToString();
                HomeLogin.mainFun = new MainFuncionalidades();
                HomeLogin.mainFun.Show();
                cerrate = 1;
                this.Close();
            }
            else
            {
                MessageBox.Show("Debe seleccionar un hotel y un rol!");
                comboBox1.Focus();
            }
            
        }
    }
}
