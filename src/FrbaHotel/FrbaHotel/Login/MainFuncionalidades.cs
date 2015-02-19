using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.Login
{
    public partial class MainFuncionalidades : Form
    {
        private int cerrate =0;
        private SqlDataReader resultado;
        public MainFuncionalidades()
        {
            InitializeComponent();
        }

        private void MainFuncionalidades_Load(object sender, EventArgs e)
        {

            string consulta = "select distinct F.denominacion from GESTION_DE_GATOS.UserXRolXHotel U,GESTION_DE_GATOS.FuncXRol FR, GESTION_DE_GATOS.Funcionalidad F,GESTION_DE_GATOS.Rol R where R.descripcion='" + Login.HomeLogin.rol + "' and R.idRol = U.rol and U.rol=FR.rol and FR.funcionalidad = F.idFuncionalidad and U.usuario = " + Login.HomeLogin.idUsuario.ToString() + " and U.hotel='" + Login.HomeLogin.hotel+"'";
            resultado = Home.BD.comando(consulta);
            while (resultado.Read() == true)
            {
                comboBox1.Items.Add(resultado.GetSqlString(0));
            }
            resultado.Close();
            consulta = "select distinct H.nombre from GESTION_DE_GATOS.Hotel H where H.idHotel = " + Login.HomeLogin.hotel.ToString();
            resultado = Home.BD.comando(consulta);
            resultado.Read();
            listBox1.Items.Add(resultado.GetSqlString(0));
            resultado.Close();
        }
        private void HandlerFuncionalidad(string funcionalidad)
        {
            switch (funcionalidad)
            {
                case "ABM ROL":
                    this.Hide();
                    ABM_de_Rol.HomeRol rol = new ABM_de_Rol.HomeRol();
                    rol.Show();
                    break;
                case "ABM USUARIO":
                    this.Hide();
                    ABM_de_Usuario.HomeUsuario usuario = new ABM_de_Usuario.HomeUsuario();
                    usuario.Show();
                    break;
                case "ABM CLIENTE":
                    this.Hide();
                    ABM_de_Cliente.HomeCliente cliente = new ABM_de_Cliente.HomeCliente();
                    cliente.Show();
                    break;
                case "ABM HOTEL":
                    this.Hide();
                    ABM_de_Hotel.HomeHotel hotel = new FrbaHotel.ABM_de_Hotel.HomeHotel();
                    hotel.Show();
                    break;
                case "ABM HABITACION":
                    this.Hide();
                    ABM_de_Habitacion.HomeHabitacion habitacion = new ABM_de_Habitacion.HomeHabitacion();
                    habitacion.Show();
                    break;
                case "ABM REGIMEN":
                    this.Hide();
                    Regimen.HomeRegimen regimen = new Regimen.HomeRegimen();
                    regimen.Show();
                    break;
                case "GENERAR/MODIFICAR RESERVA":
                    this.Hide();
                    Generar_Modificar_Reserva.HomeReserva reserva = new Generar_Modificar_Reserva.HomeReserva();
                    reserva.Show();
                    break;
                case "BAJA RESERVA":
                    this.Hide();
                    Cancelar_Reserva.Cancelacion cance = new Cancelar_Reserva.Cancelacion();
                    cance.Show();
                    break;
                case "REGISTRAR ESTADIA":
                    this.Hide();
                    Registrar_Estadia.HomeEstadia estadia = new Registrar_Estadia.HomeEstadia();
                    estadia.Show();
                    break;
                case "REGISTRAR CONSUMIBLES":
                    this.Hide();
                    Registrar_Consumible.RegistrarConsumibles consu = new Registrar_Consumible.RegistrarConsumibles();
                    consu.Show();
                    break;
                case "LISTADO ESTADISTICO":
                    this.Hide();
                    Listado_Estadistico.ListadoEstadistico listado = new Listado_Estadistico.ListadoEstadistico();
                    listado.Show();
                    break;
                default:
                    MessageBox.Show("error inesperado");
                    Application.Exit();
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(comboBox1.SelectedIndex) != -1)
            {
                HandlerFuncionalidad(comboBox1.SelectedItem.ToString());
            }
            else
            {
                MessageBox.Show("Debe seleccionar una funcionalidad");
                comboBox1.Focus();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {   
            Home.login.Show();
            cerrate=1;
            this.Close();
        }

        private void MainFuncionalidades_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cerrate == 1)
            {

            }
            else
            {
                Application.Exit();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cambioPass cambio = new cambioPass();
            cambio.Show();
        }
    }
}
