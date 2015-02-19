using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FrbaHotel.ABM_de_Hotel
{
    public partial class BajaHotel : Form
    {
        public BajaHotel()
        {
            InitializeComponent();
            dateTimePickerHasta.Value = Home.fecha;
            dateTimePickerInicio.Value = Home.fecha;
        }

        private void BajaHotel_Load(object sender, EventArgs e)
        {

        }
        private string textBoxNoVacia(TextBox cajita, string queRepresenta)
        {
            if (cajita.Text == "")
            {
                return "Debe ingresarse " + queRepresenta + "\n";
            }
            else return "";
        }
        private bool chequearCamposObligatoriosOK()
        {
            string mensajeDeError = "";
            mensajeDeError = mensajeDeError + this.textBoxNoVacia(textBoxMotivo, "un Motivo");
            mensajeDeError = mensajeDeError + this.chequearFechasOK();

            if (mensajeDeError != "")
            {
                MessageBox.Show(mensajeDeError);
                return false;
            }
            else
                return true;
        }

        private void buttonAceptar_Click(object sender, EventArgs e)
        {
            //Verificar que el hotel se encuentre vacio para esas fechas
            //Verificar que no se encuentren reservas para el rango de fechas
            if (this.chequearCamposObligatoriosOK())
            {
                string verificarQuery = "select GESTION_DE_GATOS.hotelTieneReserva('" + dateTimePickerInicio.Value.Date.ToString("yyyyMMdd HH:mm:ss") + "','"
                    + dateTimePickerHasta.Value.Date.ToString("yyyyMMdd HH:mm:ss") + "'," + Login.HomeLogin.hotel + ")";
                string huboError = FrbaHotel.Home.BD.executeAndReturn(verificarQuery);
                if (Convert.ToBoolean(huboError))
                {
                    MessageBox.Show("No se puede dar de baja el hotel en esas fechas, \n Existe alguna reserva entre ese rango de fechas.");
                }
                else
                {
                    string agregarQuery = "Insert into GESTION_DE_GATOS.BajaHotel (hotel, fecha_ini, fecha_hasta, motivo) Values (" + Login.HomeLogin.hotel + ",'" + dateTimePickerInicio.Value.Date.ToString("yyyyMMdd HH:mm:ss") + "','" + dateTimePickerHasta.Value.Date.ToString("yyyyMMdd HH:mm:ss") + "','" + textBoxMotivo.Text + "')";
                    FrbaHotel.Home.BD.executeOnly(agregarQuery);
                    MessageBox.Show("Se genero la baja del hotel en el periodo elegido");
                    this.Close();
                }
            }

        }
        private string chequearFechasOK()
        {
            if (dateTimePickerHasta.Value < dateTimePickerInicio.Value)
                return "La fecha de inicio debe ser menor o igual a la fecha de fin";
            else return "";

        }

        private void buttonCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
