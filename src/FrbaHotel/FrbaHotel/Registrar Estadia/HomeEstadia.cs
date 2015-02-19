using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FrbaHotel.Registrar_Estadia
{
    public partial class HomeEstadia : Form
    {
        int cerrate = 0;
        public HomeEstadia()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Checkin checkin = new Checkin();
            checkin.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CheckOut checkout = new CheckOut();
            checkout.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Login.HomeLogin.mainFun.Show();
            cerrate = 1;
            this.Close();
        }

        private void HomeEstadia_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cerrate == 1)
            {

            }
            else
            {
                Application.Exit();
            }
        }
    }
}
