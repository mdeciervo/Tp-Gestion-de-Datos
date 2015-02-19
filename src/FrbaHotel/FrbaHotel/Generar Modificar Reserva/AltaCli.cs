using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.Generar_Modificar_Reserva
{
    public partial class AltaCli : Form
    {
        int dias = 0;
        public static string cliente="";//numero de cliente
        DataTable tabla;
        BindingSource bSource2;
        private SqlDataReader resultado;
        SqlDataAdapter sAdapter;
        DataTable dTable;
        string habitaciones = "";
        int cant = 0;
        decimal total = 0;
        string consulta;
        public AltaCli()
        {
            InitializeComponent();
            consulta = "select distinct nombre from GESTION_DE_GATOS.Hotel";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBox1.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
            dateTimePicker1.Value = Home.fecha;
            dateTimePicker2.Value = Home.fecha;
            tabla = new DataTable();
            tabla.Columns.Add("Id");
            DataColumn column = tabla.Columns["Id"];
            column.Unique = true;                    
            tabla.Columns.Add("Precio");
            bSource2 = new BindingSource();
            bSource2.DataSource = tabla;
            //set the DataGridView DataSource
            dataGridView2.DataSource = bSource2;

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void AltaCli_Load(object sender, EventArgs e)
        {
            cliente = "";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            consulta = "EXEC GESTION_DE_GATOS.BuscarTiposHabdeHotel '" + comboBox1.SelectedItem + "'";
            resultado = Home.BD.comando(consulta);
            comboBox2.Items.Clear();
            while (resultado.Read() == true)
            {
                comboBox2.Items.Add(resultado.GetString(0));
            }
            resultado.Close();
            consulta = "EXEC GESTION_DE_GATOS.BuscarRegimenesDeHotel '" + comboBox1.SelectedItem + "'";
            resultado = Home.BD.comando(consulta);
            comboBox3.Items.Clear();
            while (resultado.Read() == true)
            {
                comboBox3.Items.Add(resultado.GetString(0));
            }
            resultado.Close();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            consulta = "select cantPersonas from GESTION_DE_GATOS.TipoHabitacion where descripcion = '" + comboBox2.SelectedItem + "'";
            resultado = Home.BD.comando(consulta);
            if (resultado.Read())
            {
                textBox1.Text = resultado.GetDecimal(0).ToString();
            }
            resultado.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                string id = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                string precio = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                string regimen = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                
                int item = dataGridView1.CurrentRow.Index;
                
                DataRow row = tabla.NewRow();
                row["Id"] = id;
                row["Precio"] = precio;
                try
                {
                    tabla.Rows.Add(row);
                    if (cant == 0)
                    {
                        habitaciones = habitaciones + id;
                        comboBox3.Text = regimen;
                        comboBox1.Enabled = false;
                        comboBox3.Enabled = false;
                        dateTimePicker1.Enabled = false;
                        dateTimePicker2.Enabled = false;
                        dias = dateTimePicker2.Value.Date.Subtract(dateTimePicker1.Value.Date).Days;
                    }
                    else
                    {
                        habitaciones = habitaciones + "," + id;
                    }
                    total = total + (((decimal)dataGridView1.CurrentRow.Cells[3].Value) * dias);
                    dataGridView1.Rows.RemoveAt(item);
                    textBox3.Text = total.ToString();
                    cant++;
                    dataGridView2.DataSource = bSource2;
                    button3_Click(null, null);
                }
                catch
                {
                    MessageBox.Show("Esa habitacion ya esta agregada");
                }
                

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            
           
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Tiene que especificar un tipo de habitacion");
                return;
            }
            if (string.IsNullOrEmpty(comboBox2.Text))
            {
                MessageBox.Show("Tiene que especificar un tipo de habitacion");
                return;
            }
            if (string.IsNullOrEmpty(comboBox1.Text))
            {
                MessageBox.Show("Tiene que especificar un hotel");
                return;
            }
            DateTime fechaDesde = Convert.ToDateTime(dateTimePicker1.Value);
            DateTime fechaHasta = Convert.ToDateTime(dateTimePicker2.Value);
            DateTime fechaHoy = Home.fecha;
            int result = DateTime.Compare(fechaDesde, fechaHasta);
            if (result >= 0)
            {
               
                MessageBox.Show("La fecha desde debe ser menor a la fecha hasta\n");
                return;
            }
            result = DateTime.Compare(fechaDesde.Date, fechaHoy);
            if (result < 0)
            {
                MessageBox.Show("La fecha desde debe ser mayor a la fecha actual\n");
                return;
            }
            string query = "EXEC GESTION_DE_GATOS.BuscarHabitacionDisponibles ";
            query = query + "'" + comboBox1.Text + "'," ;
            query = query + "'" + dateTimePicker1.Value.Date.ToString("yyyyMMdd HH:mm:ss") + "',";
            query = query + "'" + dateTimePicker2.Value.Date.ToString("yyyyMMdd HH:mm:ss") + "',";
            if (string.IsNullOrEmpty(comboBox3.Text))
            {
                query = query + "null,";
            }
            else
            {
                query = query + "'" + comboBox3.Text + "',";
            }
            query = query + "'" + comboBox2.Text + "'";
           
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
            comboBox1.ResetText();
            comboBox1.Focus();
            dateTimePicker1.Value = Home.fecha;
            dateTimePicker2.Value = Home.fecha;
            comboBox2.ResetText();
            comboBox3.ResetText();
            textBox1.Clear();
            dataGridView1.DataSource = null;
            tabla.Clear();
            dataGridView2.DataSource = bSource2;
            habitaciones = "";
            cant = 0;
            cliente = "";
            textBox2.Clear();
            total = 0;
            textBox3.Clear();
            comboBox1.Enabled = true;
            comboBox3.Enabled = true;
            dateTimePicker1.Enabled = true;
            dateTimePicker2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (habitaciones == "" || cant == 0)
            {
                MessageBox.Show("Debe seleccionar por lo menos una habitacion");
                return;
            }
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Debe seleccionar 1 cliente");
                return;
            }
            bool estado = false;
            resultado = Home.BD.comando("select habilitado from GESTION_DE_GATOS.Cliente where idCli =" + textBox2.Text);
            if (resultado.Read())
            {
                estado = resultado.GetBoolean(0);
            }
            resultado.Close();

            if (estado == false)
            {
                MessageBox.Show("Cliente Inhabilitado para realizar reservas");
                return;
            }
            //AHORA LOS INSERTS
            string insert = "EXEC GESTION_DE_GATOS.InsertarReserva ";
            insert = insert + "'" + dateTimePicker1.Value.Date.ToString("yyyyMMdd HH:mm:ss") + "',";
            insert = insert +"'"+comboBox3.Text+"',";
            insert = insert + "'" + dateTimePicker2.Value.Date.ToString("yyyyMMdd HH:mm:ss") + "',";
            insert = insert + "'" + Home.fecha.Date.ToString("yyyyMMdd HH:mm:ss") + "',";
            insert = insert +Login.HomeLogin.idUsuario+",";
            insert = insert +textBox2.Text+",";
            double ab =(dateTimePicker2.Value-dateTimePicker1.Value).TotalDays;
            insert = insert +ab.ToString()+",";
            insert = insert + Convert.ToInt32(total).ToString();
           
            decimal id = 0;
            resultado = Home.BD.comando(insert);
            if (resultado.Read() == true)
            {
                id = resultado.GetDecimal(0);
            }
            resultado.Close();

            if (id == 0)
            {

                MessageBox.Show("No se pudo generar la reserva");
                button1_Click(null, null);
                return;
            }

            string comand = "EXEC GESTION_DE_GATOS.InsertarReservaXHabitacion " + id.ToString() + ",";

            string[] strArr = null;
            int count = 0;
            char[] splitchar = { ',' };
            comand = "EXEC GESTION_DE_GATOS.InsertarReservaXHabitacion " + id.ToString() + ",";

            if (cant > 1)
            {
                strArr = null;
                count = 0;
                strArr = habitaciones.Split(splitchar);

                for (count = 0; count <= strArr.Length - 1; count++)
                {
                    resultado = Home.BD.comando(comand + strArr[count]);

                    if (!resultado.Read())
                    {

                        MessageBox.Show("No se pudo insertar la habitacion a la reserva,error en el read");
                        resultado.Close();
                        return;
                    }
                    decimal a = resultado.GetDecimal(0);
                    if (a == 0)
                    {
                        MessageBox.Show("No se pudo insertar la habitacion a la reserva, no cumplio el unique");
                        resultado.Close();
                        return;
                    }
                    resultado.Close();
                }
            }
            else
            {
                resultado = Home.BD.comando(comand + habitaciones);
                if (!resultado.Read())
                {

                    MessageBox.Show("No se pudo insertar la habitacion a la reserva,error en el read");
                    resultado.Close();
                    return;
                }
                decimal b = resultado.GetDecimal(0);
                if (b == 0)
                {
                    MessageBox.Show("No se pudo insertar la habitacion a la reserva, no cumplio el unique");
                    resultado.Close();
                    return;
                }
                resultado.Close();

            }
            MessageBox.Show("Reserva generada correctamente, su numero de reserva es: " + id.ToString());
            this.Close();


        }

        private void button4_Click(object sender, EventArgs e)
        {
            ABM_de_Cliente.Alta altaCliente = new ABM_de_Cliente.Alta("altaReservaCli");
            altaCliente.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            clientesExistentes cli = new clientesExistentes("altaReservaCli");
            cli.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void AltaCli_Activated(object sender, EventArgs e)
        {
            if (cliente != "")
            {
                textBox2.Text = cliente;
            }
        }

    }
}
