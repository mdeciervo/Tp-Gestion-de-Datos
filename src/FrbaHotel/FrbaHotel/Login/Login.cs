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
    public partial class HomeLogin : Form
    {
        public static IntermediaCliente intCli;
        public static string hotel; //es un numero, "1" por ej
        public static decimal idUsuario;
        public static string rol="";
        public static MainFuncionalidades mainFun;
        private int cerrate = 0;
        private int intentos = 0;
        public HomeLogin()
        {
            InitializeComponent();
            idUsuario = 0;
            usuario.Clear();
            clave.Clear();
            usuario.Focus();
            cerrate = 0;
            intentos = 0;
            SqlDataReader resultado;
            resultado = Home.BD.comando("EXEC GESTION_DE_GATOS.EliminarReservasAnteriores '" + Home.fecha.Date.ToString("yyyyMMdd HH:mm:ss") + "'");
            if (resultado.Read())
            {
                //se eliminaron las reservas anteriores
            }
            resultado.Close();
        }

        private void GUEST_Click(object sender, EventArgs e)
        {
            SqlDataReader resultado;
            resultado = Home.BD.comando("SELECT idUsuario FROM GESTION_DE_GATOS.Usuario where userName = 'Guest'");
            if (resultado.Read())
            {
                idUsuario = resultado.GetDecimal(0);
            }
            resultado.Close();
            string consulta = "select distinct R.descripcion from GESTION_DE_GATOS.UserXRolXHotel U,GESTION_DE_GATOS.Rol R where U.rol = R.idRol and R.estado = 1 and U.usuario = " + idUsuario.ToString() + ";";
            resultado = Home.BD.comando(consulta);
            resultado.Read();
            Login.HomeLogin.rol = resultado.GetString(0);
            resultado.Close();
            if (rol != "")
            {

                intCli = new IntermediaCliente();
                intCli.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Rol de guest inhabilitado");
            }
        }

        private void HomeLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cerrate == 1)
            {

            }
            else
            {
                Application.Exit();
            }
         }
        public void checkearUser()
        {   
            intentos++;
            SqlDataReader resultado;
            resultado = Home.BD.comando("SELECT estado,idUsuario FROM GESTION_DE_GATOS.Usuario where userName = '" + usuario.Text + "'and clave = '" + dameHash(clave.Text) + "'");
            if (resultado.Read() == true && intentos<=4)
            {
                if (resultado.GetBoolean(0) == true)
                {
                    MessageBox.Show("LOGIN CONCRETADO");
                    idUsuario = resultado.GetDecimal(1);
                    intentos = 0;
                    resultado.Close();
                    string consulta = "select COUNT(distinct UR.rol) from GESTION_DE_GATOS.UserXRolXHotel UR, GESTION_DE_GATOS.Rol R where R.estado = 1 and R.idRol = UR.rol and UR.usuario = " + idUsuario.ToString()+";";
                    resultado = Home.BD.comando(consulta);
                    resultado.Read();
                    int cant = resultado.GetInt32(0);
                    resultado.Close();
                    if(cant>1)
                    {
                        //tiene mas de un rol
                        
                        this.Hide();
                        IntermediaUsuarioConRoles intUser = new IntermediaUsuarioConRoles();
                        intUser.Show();
                        return;
                    }
                    if (cant == 1)
                    {
                        //tiene un solo rol
                        consulta = "select distinct R.descripcion from GESTION_DE_GATOS.UserXRolXHotel U,GESTION_DE_GATOS.Rol R where U.rol = R.idRol and R.estado = 1 and U.usuario = " + idUsuario.ToString() + ";";
                        resultado = Home.BD.comando(consulta);
                        resultado.Read();
                        Login.HomeLogin.rol = resultado.GetString(0);
                        resultado.Close();
                        this.Hide();
                        IntermediaUsuarioConRol intUser = new IntermediaUsuarioConRol();
                        intUser.Show();
                    }
                    else
                    {
                        MessageBox.Show("No tiene roles disponibles activos para ingresar");
                        usuario.Clear();
                        clave.Clear();
                        usuario.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("USUARIO INHABILITADO");
                    usuario.Clear();
                    clave.Clear();
                    usuario.Focus();
                }
            }
            else
            {
                MessageBox.Show("Datos Incorrectos");
                usuario.Clear();
                clave.Clear();
                usuario.Focus();
                if (intentos > 3)
                {
                    MessageBox.Show("INGRESO INHABILITADO POR CANTIDAD DE INTENTOS");
                    cerrate = 1;
                    Program.inicial.Show();
                    this.Close();
                }
            }
            resultado.Close();
        }

        private void Aceptar_Click(object sender, EventArgs e)
        {
            if (usuario.Text == "guest")
            {
                MessageBox.Show("El guest no se loguea");
                return;
            }
            checkearUser();


        }

        private void HomeLogin_Load(object sender, EventArgs e)
        {
            usuario.Clear();
            clave.Clear();
            usuario.Focus();
            cerrate = 0;
            intentos = 0;
        }

        private void Cancelar_Click(object sender, EventArgs e)
        {
            cerrate = 1;
            Program.inicial.Show();
            this.Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void HomeLogin_Shown(object sender, EventArgs e)
        {
            usuario.Clear();
            clave.Clear();
            usuario.Focus();
            cerrate = 0;
            intentos = 0;
        }

        private void HomeLogin_Activated(object sender, EventArgs e)
        {
            usuario.Clear();
            clave.Clear();
            usuario.Focus();
            cerrate = 0;
            intentos = 0;
        }

        public static String ByteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2}",b);
            }
            return hex.ToString();
        }
        public static byte[] HexStringToBytearray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[1 / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
        public static string dameHash(string clave)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(clave);
            
            System.Security.Cryptography.SHA256Managed sha256hashstring = new System.Security.Cryptography.SHA256Managed();
            byte[] hash = sha256hashstring.ComputeHash(bytes);
            string hashstring = string.Empty;
            foreach(byte x in hash)
            {
                hashstring += String.Format("{0:x2}",x);
            }
            return hashstring;
        }

        private void clave_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
