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
    public partial class ClientesEstadia : Form
    {
        string consulta;
        BindingSource bSource2;
        private SqlDataReader resultado;
        public static DataTable tabla;
        string nroReserva;
        decimal totalPers= 0;
        string nroEstadia;
        decimal idCli = 0;
        string nombre;
        string apellido;
        public static decimal persDisp = 0;
        public ClientesEstadia(string nroRes,string nroEst)
        {
            InitializeComponent();
            nroReserva = nroRes;
            nroEstadia = nroEst;
            tabla = new DataTable();
            tabla.Columns.Add("Id");
            DataColumn column = tabla.Columns["Id"];
            column.Unique = true;                    
            tabla.Columns.Add("Nombre");
            tabla.Columns.Add("Apellido");
            bSource2 = new BindingSource();
            bSource2.DataSource = tabla;
            //set the DataGridView DataSource
            dataGridView1.DataSource = bSource2;
            consulta = "select sum(T.cantPersonas) from GESTION_DE_GATOS.ReservaXHabitacion RH,GESTION_DE_GATOS.Habitacion H,GESTION_DE_GATOS.TipoHabitacion T where RH.habitacion = H.idHabitacion and T.codigo=H.tipo and RH.reserva = " + nroReserva;
            resultado = Home.BD.comando(consulta);
            if (resultado.Read())
            {
                textBox2.Text = resultado.GetDecimal(0).ToString();
                totalPers = resultado.GetDecimal(0);
                resultado.Close();
            }
            else
            {
                resultado.Close();
                MessageBox.Show("La reserva no tiene habitaciones");
                this.Close();
            }
            consulta = "select C.idCli,C.nombre,C.apellido from GESTION_DE_GATOS.Reserva R,GESTION_DE_GATOS.Cliente C where R.cliente = C.idCli and R.idReserva = " + nroReserva;
            resultado = Home.BD.comando(consulta);
            resultado.Read();          
            textBox3.Text = resultado.GetString(1) + " " + resultado.GetString(2);
            persDisp = totalPers - 1;
            textBox4.Text = (persDisp).ToString();
            idCli = resultado.GetDecimal(0);
            nombre = resultado.GetString(1);
            apellido = resultado.GetString(2);
            DataRow row = tabla.NewRow();
            row["Id"] = idCli;
            row["Nombre"] = nombre;
            row["Apellido"] = apellido;
            tabla.Rows.Add(row);
            textBox1.Text = nroReserva;
            resultado.Close();

        }

        private void ClientesEstadia_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (persDisp > 0)
            {
                ABM_de_Cliente.Alta clialta = new ABM_de_Cliente.Alta("altaEstadia");
                clialta.Show();
            }
            else
            {
                MessageBox.Show("No se pueden agregar mas clientes para esta estadia");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (persDisp > 0)
            {
                cliExistentes cliex = new cliExistentes();
                cliex.Show();
            }
            else
            {
                MessageBox.Show("No se pueden agregar mas clientes para esta estadia");
            }
           
        }

        private void ClientesEstadia_Activated(object sender, EventArgs e)
        {
            bSource2.DataSource = tabla;
            textBox4.Text = persDisp.ToString();


        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
               int item = dataGridView1.CurrentRow.Index;
               if (dataGridView1.CurrentRow.Cells[1].Value.ToString() == idCli.ToString())
               {
                   MessageBox.Show("No se puede borrar el cliente que hizo la reserva");
                   return;
               }
               persDisp++;
               tabla.Rows.RemoveAt(item);
               dataGridView1.DataSource = tabla;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
            foreach (DataRow fila in tabla.Rows)
            {
                resultado = Home.BD.comando("EXEC GESTION_DE_GATOS.RegistrarEstadiaXCliente " + fila["Id"].ToString() + ","+nroEstadia);
                if (resultado.Read())
                {
                    if (resultado.GetDecimal(0) == 0)
                    {
                        MessageBox.Show("Error. El cliente ya estaba agregado");
                    }
                }
                else
                {
                    MessageBox.Show("Error. El cliente ya estaba agregado");
                }
                resultado.Close();
            }
            MessageBox.Show("El proceso de carga de clientes finalizo correctamente");
            Distribucion_Clientes distri = new Distribucion_Clientes(tabla, nroReserva, nroEstadia);
            distri.Show();
            
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tabla.Clear();
            persDisp = totalPers - 1;
            DataRow row = tabla.NewRow();
            row["Id"] = idCli;
            row["Nombre"] = nombre;
            row["Apellido"] = apellido;
            tabla.Rows.Add(row);


        }
    }
}