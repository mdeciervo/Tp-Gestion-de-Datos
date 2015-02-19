using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.Facturar_Estadia
{
    public partial class Facturacion : Form
    {
        DataTable tablaDias;
        DataTable consumibles;
        string consulta;
        SqlDataReader resultado;
        BindingSource bSource2;
        public Facturacion(string idEstadia)
        {
            InitializeComponent();
            textBox1.Text = idEstadia;
            consulta = "select reserva from GESTION_DE_GATOS.Estadia where idEstadia = " + idEstadia;
            resultado = Home.BD.comando(consulta);
            resultado.Read();
            textBox2.Text = resultado.GetDecimal(0).ToString();
            resultado.Close();
            consulta = "select distinct descripcion from GESTION_DE_GATOS.FormaDePago";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBox1.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
            consulta = "select ingreso,salida,dias_sobran,cantidadNoches,precioPorNoche from GESTION_DE_GATOS.Estadia where idEstadia = " + idEstadia;
            resultado = Home.BD.comando(consulta);
            resultado.Read();
            DateTime ingreso = resultado.GetDateTime(0);
            DateTime salida = resultado.GetDateTime(1);
            decimal diasSobran = resultado.GetDecimal(2);
            decimal cantNoches = resultado.GetDecimal(3);
            decimal precio = resultado.GetDecimal(4);
            textBox4.Text = ingreso.ToShortDateString();
            textBox5.Text = ingreso.AddDays(Convert.ToDouble(cantNoches+diasSobran)).ToShortDateString();
            textBox6.Text = salida.ToShortDateString();
            resultado.Close();
            tablaDias = new DataTable();
            tablaDias.Columns.Add("Fecha");
            tablaDias.Columns.Add("Precio");
            tablaDias.Columns.Add("Descripcion");
            decimal auxcantnoches = cantNoches;
            decimal auxdiassobran = diasSobran;
            DataRow row = tablaDias.NewRow();
            while (cantNoches >0)
            {
                row = tablaDias.NewRow();
                                     
                row["Fecha"] = ingreso;
                row["Precio"] = precio;
                row["Descripcion"] = "SE ALOJO";
                tablaDias.Rows.Add(row);
                ingreso = ingreso.AddDays(1);
                cantNoches--;
            }
            while (diasSobran > 0)
            {
                row = tablaDias.NewRow();
                
                row["Fecha"] = ingreso;
                row["Precio"] = precio;
                row["Descripcion"] = "NO SE ALOJO";
                tablaDias.Rows.Add(row);
                ingreso = ingreso.AddDays(1);
                diasSobran--;
            }
            bSource2 = new BindingSource();
            bSource2.DataSource = tablaDias;
            //set the DataGridView DataSource
            dataGridView1.DataSource = bSource2;
            textBox7.Text = (precio * (auxdiassobran + auxcantnoches)).ToString();


            //aca iria lo de los consumibles, que hay que utilizar itemFactura

            string query = "select I.cantidad Cantidad, I.descripcion Descripcion, I.monto Monto from GESTION_DE_GATOS.ItemFactura I,GESTION_DE_GATOS.Factura F where I.factura = F.numero and I.descripcion != 'Estadia' and F.estadia = "+idEstadia;
            SqlDataAdapter sAdapter = FrbaHotel.Home.BD.dameDataAdapter(query);
            consumibles = FrbaHotel.Home.BD.dameDataTable(sAdapter);
            //BindingSource to sync DataTable and DataGridView
            BindingSource bSource = new BindingSource();
            //set the BindingSource DataSource
            bSource.DataSource = consumibles;
            //set the DataGridView DataSource
            dataGridView2.DataSource = bSource;

            consulta = "select C.precio from GESTION_DE_GATOS.ConsumibleXEstadia CE, GESTION_DE_GATOS.Consumibles C where CE.consumible = C.idConsumible and CE.estadia = " + idEstadia;
            resultado = Home.BD.comando(consulta);
            if (resultado.Read()==true)
            {
                resultado.Close();
                consulta = "select sum(C.precio) from GESTION_DE_GATOS.ConsumibleXEstadia CE, GESTION_DE_GATOS.Consumibles C where CE.consumible = C.idConsumible and CE.estadia = " + idEstadia;
                resultado = Home.BD.comando(consulta);
                resultado.Read();
                textBox9.Text = resultado.GetDecimal(0).ToString();
            }
            else
            {   
                textBox9.Text = 0.ToString();
            }
            resultado.Close();

            consulta = "select R.regimen from GESTION_DE_GATOS.Reserva R, GESTION_DE_GATOS.Estadia E where E.reserva= R.idReserva and E.idEstadia = " + idEstadia;
            resultado = Home.BD.comando(consulta);
            resultado.Read();
            decimal regimen = resultado.GetDecimal(0);
            resultado.Close();
            if (regimen == 3)
            {
                textBox8.Text = "-"+textBox9.Text;
                textBox3.Text = textBox7.Text;
            }
            else 
            { 
               textBox8.Text = 0.ToString();
               decimal resu = Convert.ToDecimal(textBox7.Text) + Convert.ToDecimal(textBox9.Text);
               textBox3.Text = (resu).ToString();
            }



        }

        private void Facturacion_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBox1.Text))
            {
                MessageBox.Show("Debe seleccionar una Forma de pago");
                return;
            }
            consulta = "select idFormaDePago from GESTION_DE_GATOS.FormaDePago where descripcion = '" + comboBox1.Text +"'";
            resultado = Home.BD.comando(consulta);
            resultado.Read();
            decimal fp = resultado.GetDecimal(0);
            resultado.Close();
            consulta = "EXEC GESTION_DE_GATOS.ModificarFactura " + textBox1.Text +","+fp.ToString()+",'"+Home.fecha.Date+"'";
            resultado = Home.BD.comando(consulta);
            resultado.Read();
            decimal factura = resultado.GetDecimal(0);
            if (factura == 0)
            {
                resultado.Close();
                MessageBox.Show("error al insertar factura, ya esta generada");
                return;
            }
            resultado.Close();
            if (fp == 2)
            {
                //tarjeta de credito
                Tarjeta tarj = new Tarjeta(factura.ToString());
                tarj.Show();
            }
            MessageBox.Show("Se ha generado la factura correctamente");
            this.Close();
        }
    }
}
