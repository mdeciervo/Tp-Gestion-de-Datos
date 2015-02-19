using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.Registrar_Estadia
{
    public partial class Distribucion_Clientes : Form
    {
        string consulta;
        SqlDataReader resultado;
        DataTable tabla;
        string estadia;
        public Distribucion_Clientes(DataTable tablaP,string nroReserva, string nroEstadia)
        {
            InitializeComponent();
            tabla = tablaP;
            estadia = nroEstadia;
            dataGridView1.DataSource = tabla;
            consulta= "select habitacion from GESTION_DE_GATOS.ReservaXHabitacion where reserva = "+nroReserva;
            resultado = Home.BD.comando(consulta);
            while( resultado.Read())
            {
                comboBox1.Items.Add(resultado.GetDecimal(0));
            }
            resultado.Close();
           


        }

        private void Distribucion_Clientes_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            consulta= "select H.numero,H.piso,T.descripcion,T.cantPersonas from GESTION_DE_GATOS.Habitacion H, GESTION_DE_GATOS.TipoHabitacion T where H.tipo = T.codigo and H.idHabitacion = "+comboBox1.Text;
            resultado = Home.BD.comando(consulta);
            if( resultado.Read())
            {
                textBox1.Text = resultado.GetDecimal(0).ToString();
                textBox2.Text = resultado.GetDecimal(1).ToString();
                textBox4.Text = resultado.GetDecimal(3).ToString();
                textBox3.Text = resultado.GetString(2);
            }
            else
            {
                MessageBox.Show("Error la habitacion no existe");
            }
            resultado.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if(Convert.ToInt32(comboBox1.SelectedIndex) == -1)
                {
                    MessageBox.Show("Debe seleccionar una habitacion");
                    return;

                }
                int index = dataGridView1.CurrentRow.Index;
                consulta = "select count(*) from GESTION_DE_GATOS.ClienteXEstadia where estadia = "+estadia +" and habitacion = "+comboBox1.Text;
                resultado = Home.BD.comando(consulta);
                resultado.Read();
                int aux = resultado.GetInt32(0);
                resultado.Close();
                if(aux < Convert.ToInt32(textBox4.Text))
                {
                    consulta = "EXEC GESTION_DE_GATOS.ModificarClienteXEstadia "+comboBox1.Text+ ","+dataGridView1.CurrentRow.Cells[1].Value.ToString()+","+estadia;
                    resultado = Home.BD.comando(consulta);
                    resultado.Read();
                    if(resultado.GetDecimal(0)==1)
                    {
                        MessageBox.Show("El cliente se agrego correctamente");
                        tabla.Rows.RemoveAt(index);
                        dataGridView1.DataSource = tabla;
                        resultado.Close();
                    }
                    else
                    {
                        resultado.Close();
                        MessageBox.Show("El cliente ya estaba agregado a esa estadia");
                        return;
                    }
                }
                else
                {   
                    MessageBox.Show("Esa habitacion ya esta llena");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(tabla.Rows.Count==0)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Debe asignar los clientes restantes a una habitacion");
            }
        }
    }
}
