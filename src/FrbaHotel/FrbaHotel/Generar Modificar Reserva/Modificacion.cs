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
    public partial class Modificacion : Form
    {
        public static string cliente = "";//numero de cliente
        DataTable tabla;
        BindingSource bSource2;
        private SqlDataReader resultado;
        SqlDataAdapter sAdapter;
        DataTable dTable;
        string habitaciones = "";
        string habitacionesLocas = "";
        int cant = 0;
        int cantN = 0;
        decimal total = 0;
        string consulta;
        int cambio = 0;
        int dias = 0;
        public Modificacion(string idReserva)
        {
            InitializeComponent();
            tabla = new DataTable();
            textBox5.Text = idReserva;
            consulta = "SELECT H.nombre,R.fecha_inicio,R.fecha_hasta,RE.descripcion,R.cliente,R.costoTotal from GESTION_DE_GATOS.Reserva R,GESTION_DE_GATOS.Hotel H, GESTION_DE_GATOS.Regimen RE,GESTION_DE_GATOS.Habitacion Ha,GESTION_DE_GATOS.ReservaXHabitacion RH where RE.codigo = R.regimen and RH.reserva = R.idReserva and RH.habitacion = Ha.idHabitacion and Ha.hotel = H.idHotel and R.idReserva = " + idReserva;
            resultado = Home.BD.comando(consulta);
            if (resultado.Read() == true)
            {
                textBox3.Text = resultado.GetString(0);
                dateTimePicker1.Value = resultado.GetDateTime(1);
                dateTimePicker2.Value = resultado.GetDateTime(2);
                comboBox3.Text = resultado.GetString(3);
                textBox2.Text = resultado.GetDecimal(4).ToString();
                textBox4.Text = resultado.GetDecimal(5).ToString();
                resultado.Close();
            }
            else
            {
                resultado.Close();
                MessageBox.Show("error en la reserva");
                this.Close();
            }


            consulta = "EXEC GESTION_DE_GATOS.BuscarTiposHabdeHotel '" + textBox3.Text + "'";
            resultado = Home.BD.comando(consulta);
            
            while (resultado.Read() == true)
            {
                comboBox2.Items.Add(resultado.GetString(0));
            }
            resultado.Close();
            consulta = "EXEC GESTION_DE_GATOS.BuscarRegimenesDeHotel '" + textBox3.Text + "'";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBox3.Items.Add(resultado.GetString(0));
            }
            resultado.Close();
            
            tabla.Columns.Add("Id");
            DataColumn column = tabla.Columns["Id"];
            column.Unique = true;
            tabla.Columns.Add("Precio");

            consulta = "EXEC GESTION_DE_GATOS.BuscarHabitacionDeReserva "+idReserva;
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                DataRow row = tabla.NewRow();
                row["Id"] = resultado.GetDecimal(0).ToString();
                row["Precio"] = resultado.GetDecimal(1).ToString();
                tabla.Rows.Add(row);
                if (cantN == 0)
                {
                    habitacionesLocas = habitacionesLocas +resultado.GetDecimal(0).ToString();
                }
                else
                {
                    habitacionesLocas = habitacionesLocas + "," + resultado.GetDecimal(0).ToString();
                    
                }
                cantN++;
                total = total + resultado.GetDecimal(1);
            }
            cambio = 0;
            resultado.Close();
            bSource2 = new BindingSource();
            bSource2.DataSource = tabla;
            //set the DataGridView DataSource
            dataGridView2.DataSource = bSource2;
           
        }

        private void Modificacion_Load_1(object sender, EventArgs e)
        {

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
                        if (string.IsNullOrEmpty(comboBox3.Text))
                        {
                            comboBox3.Text = regimen;
                        }
                        
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
                    textBox4.Text = total.ToString();
                    cant++;
                    dataGridView2.DataSource = bSource2;
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
            if (string.IsNullOrEmpty(textBox3.Text))
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
            query = query + "'" + textBox3.Text + "',";
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
            textBox3.Focus();
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
            textBox4.Clear();
            total = 0;
            cambio = 1;
            comboBox3.Enabled = true;
            dateTimePicker1.Enabled = true;
            dateTimePicker2.Enabled = true;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            dataGridView2.DataSource = null;
            cant = 0;
            habitaciones = "";
            cambio = 1;
            tabla.Clear();
            total = 0;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dataGridView2.DataSource = null;
            cant = 0;
            habitaciones = "";
            cambio = 1;
            total = 0;
            tabla.Clear();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            dataGridView2.DataSource = null;
            cant = 0;
            total = 0;
            habitaciones = "";
            cambio = 1;
            tabla.Clear();
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
            string insert = "EXEC GESTION_DE_GATOS.ModificarReserva ";
            insert = insert + textBox5.Text + ",";
            insert = insert + "'" + dateTimePicker1.Value.Date.ToString("yyyyMMdd HH:mm:ss") + "',";
            insert = insert + "'" + comboBox3.Text + "',";
            insert = insert + "'" + dateTimePicker2.Value.Date.ToString("yyyyMMdd HH:mm:ss") + "',";
            insert = insert + "'" + Home.fecha.Date.ToString("yyyyMMdd HH:mm:ss") + "',";
            insert = insert + Login.HomeLogin.idUsuario + ",";
            double ab = (dateTimePicker2.Value - dateTimePicker1.Value).TotalDays;
            insert = insert + ab.ToString() + ",";
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

                MessageBox.Show("No se pudo modificar la reserva");
                button1_Click(null, null);
                return;
            }
            else
            {
                id = Convert.ToInt32(textBox5.Text);
            }
            string comand;
            string[] strArr = null;
            int count = 0;
            char[] splitchar = { ',' };
            if (cambio == 1)
            {
                comand = "EXEC GESTION_DE_GATOS.EliminarHabitacionesDeReserva " + id.ToString() + ",";
                

                if (cantN > 1)
                {

                    strArr = habitacionesLocas.Split(splitchar);

                    for (count = 0; count <= strArr.Length - 1; count++)
                    {
                        resultado = Home.BD.comando(comand + strArr[count]);

                        if (!resultado.Read())
                        {

                            MessageBox.Show("No se pudo borrar la habitacion a la reserva,error en el read");
                            resultado.Close();
                            return;
                        }
                        decimal a = resultado.GetDecimal(0);
                        if (a == 0)
                        {
                            MessageBox.Show("No se pudo borrar la habitacion a la reserva, no cumplio el unique");
                            resultado.Close();
                            return;
                        }
                        resultado.Close();
                    }
                }
                else
                {
                    resultado = Home.BD.comando(comand + habitacionesLocas);
                    if (!resultado.Read())
                    {

                        MessageBox.Show("No se pudo borrar la habitacion a la reserva,error en el read");
                        resultado.Close();
                        return;
                    }
                    decimal b = resultado.GetDecimal(0);
                    if (b == 0)
                    {
                        MessageBox.Show("No se pudo borrar la habitacion a la reserva, no cumplio el unique");
                        resultado.Close();
                        return;
                    }
                    resultado.Close();
                }
            }
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

            string con = "select C.nombre,C.apellido,T.descripcion,C.nroDoc,C.mail,C.telefono,P.nombre,C.direccion,C.fecha_nac,C.habilitado from GESTION_DE_GATOS.Cliente C,GESTION_DE_GATOS.Pais P,GESTION_DE_GATOS.TiposDoc T where C.tipoDoc = T.idTipoDoc and C.nacionalidad = P.idPais and C.idCli = " + textBox2.Text;
            resultado = Home.BD.comando(con);
            resultado.Read();
            decimal id = Login.HomeLogin.idUsuario;
            string nombre = resultado.GetString(0);
            string apellido = resultado.GetString(1);
            string tipoDoc = resultado.GetString(2);
            string nroDoc = resultado.GetDecimal(3).ToString();
            string mail = resultado.GetString(4);
            string telefono = resultado.GetDecimal(5).ToString();
            string nacionalidad = resultado.GetString(6);
            string direccion = resultado.GetDecimal(7).ToString();
            string fecha_nac = resultado.GetDateTime(8).ToString();
            string habilitado = 1.ToString();
            if (resultado.GetBoolean(9))
            {
                habilitado = 1.ToString();
            }
            else
            {
                habilitado = 0.ToString();
            }

            resultado.Close();
            ABM_de_Cliente.Modificacion modi = new ABM_de_Cliente.Modificacion(id, nombre, apellido, tipoDoc, nroDoc, mail, telefono, nacionalidad, direccion, fecha_nac, habilitado);
            modi.Show();
        }

    }
}
