using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.Cancelar_Reserva
{
    public partial class Cancelacion : Form
    {
        decimal noshow;
        SqlDataReader resultado;
        decimal canceUser;
        decimal cancelCli;
        public Cancelacion()
        {
            InitializeComponent();
            textBox1.Clear();
            textBox1.Focus();
            noshow = 0;
            resultado = Home.BD.comando("Select idEstado from GESTION_DE_GATOS.Estado where descripcion = 'RESERVA CANCELADA POR NO-SHOW'");
            resultado.Read();
            noshow = resultado.GetDecimal(0);
            resultado.Close();
            canceUser = 0;
            resultado = Home.BD.comando("Select idEstado from GESTION_DE_GATOS.Estado where descripcion = 'RESERVA CANCELADA POR RECEPCION'");
            resultado.Read();
            canceUser = resultado.GetDecimal(0);
            resultado.Close();
            cancelCli = 0;
            resultado = Home.BD.comando("Select idEstado from GESTION_DE_GATOS.Estado where descripcion = 'RESERVA CANCELADA POR CLIENTE'");
            resultado.Read();
            cancelCli = resultado.GetDecimal(0);
            resultado.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Cancelacion_Load(object sender, EventArgs e)
        {

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

                if (fechaIni.Date <=Home.fecha.Date)
                {
                    MessageBox.Show("La reserva no se puede cancelar porque ya paso el plazo para cancelar");
                    return;
                }
                if (estado == noshow || estado == cancelCli || estado == canceUser)
                {
                    MessageBox.Show("La reserva ya se encuentra cancelada");
                    return;
                }
                string cancelacion = "";
                if (Login.HomeLogin.idUsuario == 1)
                {
                    //cancelo la reserva con estado cliente
                    cancelacion = "exec GESTION_DE_GATOS.CancelarReserva ";
                    cancelacion = cancelacion + textBox1.Text + ",";
                    cancelacion = cancelacion + cancelCli + ",";
                    cancelacion = cancelacion + Login.HomeLogin.idUsuario + ",";
                    cancelacion = cancelacion + "'" + Home.fecha.Date.ToString("yyyyMMdd HH:mm:ss") + "',";
                    cancelacion = cancelacion + "'" + textBox2.Text + "'";
                }
                else
                {
                    //cancelo la reserva con estado user
                    cancelacion = "exec GESTION_DE_GATOS.CancelarReserva ";
                    cancelacion = cancelacion + textBox1.Text + ",";
                    cancelacion = cancelacion + canceUser+ ",";
                    cancelacion = cancelacion + Login.HomeLogin.idUsuario + ",";
                    cancelacion = cancelacion + "'" + Home.fecha.Date.ToString("yyyyMMdd HH:mm:ss") + "',";
                    cancelacion = cancelacion + "'" + textBox2.Text + "'";
                }
                resultado = Home.BD.comando(cancelacion);
                if(resultado.Read())
                {
                    if (resultado.GetDecimal(0) == 1)
                    {
                        MessageBox.Show("La reserva se cancelo correctamente");
                    }
                    else
                    {
                        MessageBox.Show("Erro al cancelar la reserva, ya se encontraba cancelada");
                    }
                    resultado.Close();
                    this.Close();
                }
                resultado.Close();
            }
            else
            {
                resultado.Close();
                MessageBox.Show("Numero de reserva invalido");
                return;
            }
        }

        private void Cancelacion_FormClosing(object sender, FormClosingEventArgs e)
        {
            Login.HomeLogin.mainFun.Show();
        }


    }
}
