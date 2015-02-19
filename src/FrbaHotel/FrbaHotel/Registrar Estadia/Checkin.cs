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
    public partial class Checkin : Form
    {
        SqlDataReader resultado;
        decimal resok;
        decimal resmodif;
        decimal estadia = 0;
        public Checkin()
        {
            InitializeComponent();
            textBox1.Clear();
            textBox1.Focus();
            
            resok = 0;
            resultado = Home.BD.comando("Select idEstado from GESTION_DE_GATOS.Estado where descripcion = 'RESERVA CORRECTA'");
            resultado.Read();
            resok = resultado.GetDecimal(0);
            resultado.Close();
            resmodif = 0;
            resultado = Home.BD.comando("Select idEstado from GESTION_DE_GATOS.Estado where descripcion = 'RESERVA MODIFICADA'");
            resultado.Read();
            resmodif = resultado.GetDecimal(0);
            resultado.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Debe ingresar un numero de reserva");
                return;
            }
            resultado = Home.BD.comando("Select idReserva,estado,fecha_inicio from GESTION_DE_GATOS.Reserva where idReserva = " + textBox1.Text);
            if (resultado.Read() == true)
            {
                decimal estado = resultado.GetDecimal(1);
                DateTime fechaIni = resultado.GetDateTime(2);
                resultado.Close();
                //busco los estado

                if (fechaIni.Date < Home.fecha.Date)
                {
                    MessageBox.Show("No se puede hacer el check in porque ya vencio la fecha de check in");
                    return;
                }
                if (fechaIni.Date > Home.fecha.Date)
                {
                    MessageBox.Show("No se puede hacer el check in porque no es la fecha todavia");
                    return;
                }
                if (estado != resmodif && estado != resok)
                {
                    MessageBox.Show("La reserva no tiene el estado Correcto. Posiblemente cancelada");
                    return;
                }
                resultado = Home.BD.comando("select H.idHotel from GESTION_DE_GATOS.Habitacion Ha,GESTION_DE_GATOS.Hotel H,GESTION_DE_GATOS.ReservaXHabitacion RH where RH.habitacion = Ha.idHabitacion and Ha.hotel = H.idHotel and RH.reserva = "+textBox1.Text);
                decimal hotel = 0;
                if(resultado.Read())
                {
                    hotel = resultado.GetDecimal(0);
                    resultado.Close();
                }
                else
                {
                    resultado.Close();
                    MessageBox.Show("Error. La reserva no tiene asignadas habitaciones");
                    return;
                }
                if(hotel.ToString()!=Login.HomeLogin.hotel)
                {
                    MessageBox.Show("Error. Solo pueden hacer checks-in los empleados del hotel donde se hizo la reserva");
                    return;
                }
                //aca se llama a insertar ESTADIA
                string cancelacion = "EXEC GESTION_DE_GATOS.registrarCheckinEstadia ";
                cancelacion = cancelacion + textBox1.Text + ",";
                cancelacion = cancelacion + Login.HomeLogin.idUsuario + ",";
                cancelacion = cancelacion + "'" + Home.fecha.Date.ToString("yyyyMMdd HH:mm:ss") + "'";
                resultado = Home.BD.comando(cancelacion);
                resultado.Read();
                estadia = resultado.GetDecimal(0);
                resultado.Close();
                if (estadia != 0)
                {
                    MessageBox.Show("Se ha realizado el check-in correctamente");
                    //SE GENERA LA FACTURA
                    string factu = "EXEC GESTION_DE_GATOS.generarFactura " + estadia.ToString() + ",1,'" + Home.fecha.Date.ToString("yyyyMMdd HH:mm:ss") + "'";
                    resultado = Home.BD.comando(factu);
                    resultado.Read();
                    decimal factura = resultado.GetDecimal(0);
                    if (factura == 0)
                    {
                        resultado.Close();
                        MessageBox.Show("Error al insertar factura, ya esta generada");
                        return;
                    }
                    resultado.Close();
                    //ACA SERIA LO DE CLIENTESXESTADIA
                    ClientesEstadia cliest = new ClientesEstadia(textBox1.Text,estadia.ToString());
                    cliest.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo realizar la operacion, la estadia ya esta registrada");
                    return;
                }
                
                
                
            }
            else
            {
                resultado.Close();
                MessageBox.Show("Numero de reserva invalido");
                return;
            }
        }
    }
}
