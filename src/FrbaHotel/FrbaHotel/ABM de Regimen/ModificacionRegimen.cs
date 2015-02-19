using System;
using System.Windows.Forms;

namespace FrbaHotel.Regimen
{
    public partial class Modificacion : Form
    {
        private String query;

        public Modificacion(string accion , string queryParam)
        {
            InitializeComponent();
            buttonAceptar.Text = accion;
            query = queryParam;
            textBox1.Text = FrbaHotel.Home.BD.executeAndReturn("SELECT IDENT_CURRENT ('Gestion_DE_Gatos.Regimen')+1");
        }

        public Modificacion(ABM_de_Regimen.Regimen regimen, string accion, string queryParam)
        {
            InitializeComponent();
            buttonAceptar.Text = accion;
            query = queryParam;
            textBox1.Text = Convert.ToString(regimen.codigo);
            textBox2.Text = regimen.descripcion;
            textBox3.Text = Convert.ToString(regimen.precio);
            checkBox1.Checked = regimen.estado;
            if (accion.Equals("Borrar")) {
                textBox2.ReadOnly = true;
                textBox3.ReadOnly = true;
                checkBox1.Enabled = false;
            }
        }

        private void buttonAceptar_Click(object sender, EventArgs e)
        {
            switch (buttonAceptar.Text)
            {
                case "Agregar":
                    query = query + "('" + textBox2.Text + "','" + textBox3.Text + "','" + checkBox1.Checked + "');";
                    FrbaHotel.Home.BD.executeOnly(query);
                    break;
                case "Borrar":
                    query = query + textBox1.Text;
                    FrbaHotel.Home.BD.executeOnly(query);
                    break;
                case "Modificar":
                    query = query + "descripcion = '" + textBox2.Text + "', precio = " + textBox3.Text.Replace(',','.') + ", estado = '" + checkBox1.Checked + "' WHERE codigo=" + textBox1.Text;
                    FrbaHotel.Home.BD.executeOnly(query);
                    break;
                default:
                    break;
            }
            this.Close();
        }

        private void buttonCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
