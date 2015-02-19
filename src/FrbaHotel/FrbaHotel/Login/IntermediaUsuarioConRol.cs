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
    public partial class IntermediaUsuarioConRol : Form
    {
        private int cerrate = 0;
        private SqlDataReader resultado;
        public IntermediaUsuarioConRol()
        {
            InitializeComponent();
            
        }

        private void IntermediaUsuarioConRol_Load(object sender, EventArgs e)
        {
            string consulta = "select H.idHotel from GESTION_DE_GATOS.UserXRolXHotel U,GESTION_DE_GATOS.Hotel H where U.hotel=H.idHotel and U.usuario = " + Login.HomeLogin.idUsuario.ToString() + " order by H.idHotel";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBox1.Items.Add(resultado.GetDecimal(0));
            }
            resultado.Close();
            consulta = "select H.idHotel Id, H.nombre Nombre from GESTION_DE_GATOS.UserXRolXHotel U,GESTION_DE_GATOS.Hotel H where U.hotel=H.idHotel and U.usuario = " + Login.HomeLogin.idUsuario.ToString() + " order by H.idHotel";
            
            DataTable result = Home.BD.consulta(consulta);
            dataGridView1.DataSource = result;
        }

        private void IntermediaUsuarioConRol_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cerrate == 1)
            {

            }
            else
            {
                Application.Exit();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(comboBox1.SelectedIndex) != -1)
            {
                Login.HomeLogin.hotel = comboBox1.SelectedItem.ToString();
                HomeLogin.mainFun = new MainFuncionalidades();
                HomeLogin.mainFun.Show();
                cerrate = 1;
                this.Close();
            }
            else
            {
                MessageBox.Show("Debe seleccionar un hotel!");
                comboBox1.Focus();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

       

     
    }
}
