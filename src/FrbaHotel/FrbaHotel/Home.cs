using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FrbaHotel
{   
    public partial class Home : Form
    {
        public static DateTime fecha;
        public static SQLConnector BD = new SQLConnector();
        public static Login.HomeLogin login;
        public Home()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            fecha = dateTimePicker1.Value;
         //   MessageBox.Show(fecha.ToShortDateString());
            this.Hide();
            login = new Login.HomeLogin();
            login.Show();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            
        }

        private void Home_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        

    }
}
