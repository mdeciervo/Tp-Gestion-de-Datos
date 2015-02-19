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
    public partial class mostraDir : Form
    {
        public mostraDir(string dir)
        {
            InitializeComponent();
            string consulta = "select D.calle,D.numero,D.piso,D.depto,D.ciudad,P.nombre from GESTION_DE_GATOS.Direccion D,GESTION_DE_GATOS.Pais P where D.pais = P.idPais and idDir ="+dir;
            SqlDataReader resultado = Home.BD.comando(consulta);
            if(resultado.Read())
            {
                textBox1.Text = resultado.GetString(0);
                textBox2.Text = resultado.GetDecimal(1).ToString();
                if (resultado.IsDBNull(2))
                {
                }
                else
                {
                    textBox3.Text = resultado.GetDecimal(2).ToString();
                }
                if (resultado.IsDBNull(3))
                {
                }
                else
                {
                    textBox4.Text = resultado.GetString(3);
                }
                if (resultado.IsDBNull(4))
                {
                }
                else
                {
                    textBox5.Text = resultado.GetString(4);
                }
                textBox6.Text = resultado.GetString(5);
            }
            resultado.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void mostraDir_Load(object sender, EventArgs e)
        {

        }
    }
}
