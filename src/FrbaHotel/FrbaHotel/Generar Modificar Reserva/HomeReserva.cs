using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FrbaHotel.Generar_Modificar_Reserva
{
    public partial class HomeReserva : Form
    {
       
        int cerrate = 0;
        public HomeReserva()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
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
            if (Login.HomeLogin.rol == "GUEST")
            {
                AltaCli alta = new AltaCli();
                alta.Show();
            }
            else
            {
                AltaUser alta = new AltaUser();
                alta.Show();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ModifRes modi = new ModifRes();
            modi.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Login.HomeLogin.rol == "GUEST")
            {
                Login.HomeLogin.intCli.Show();
                
            }
            else
            {
            Login.HomeLogin.mainFun.Show();
            
            }
            cerrate = 1;
            this.Close();
        }
        
    }
}
