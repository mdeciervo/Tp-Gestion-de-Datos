using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrbaHotel.ABM_de_Rol
{
    public partial class Modificacion : Form
    {
        string ID;
        SqlDataReader resultado;
        public Modificacion(string id, string rol, string estado)
        {
            InitializeComponent();
            textBox1.Text = rol;
            ID = id;
            comboBox1.Items.Add("Activo");
            comboBox1.Items.Add("Inactivo");
            if (estado == "True")
            {
                comboBox1.Text = "Activo";
            }
            else
            {
                comboBox1.Text = "Inactivo";
            }
            string consulta = "select F.denominacion from GESTION_DE_GATOS.FuncXRol FR,GESTION_DE_GATOS.Funcionalidad F where F.idFuncionalidad =FR.funcionalidad and FR.rol = " + id;
            resultado = Home.BD.comando(consulta);
            comboBox3.Items.Add("Ninguna");
            comboBox2.Items.Add("Ninguna");
            while (resultado.Read())
            {
                comboBox3.Items.Add(resultado.GetString(0));
            }
            resultado.Close();
            consulta = "select distinct denominacion from GESTION_DE_GATOS.Funcionalidad except select F.denominacion from GESTION_DE_GATOS.FuncXRol FR,GESTION_DE_GATOS.Funcionalidad F where F.idFuncionalidad =FR.funcionalidad and FR.rol =" + id;
            resultado = Home.BD.comando(consulta);
            while (resultado.Read())
            {
                comboBox2.Items.Add(resultado.GetString(0));
            }
            resultado.Close();
            textBox1.Focus();

        }

        private void Modificacion_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            comboBox1.ResetText();
            comboBox2.ResetText();
            comboBox3.ResetText();
            textBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("El nombre tiene que estar completo");
                return;
            }
            if (string.IsNullOrEmpty(comboBox1.Text))
            {
                MessageBox.Show("El estado tiene que estar completo");
                return;
            }
            string consulta;
            if (comboBox1.Text == "Activo")
            {
                consulta = "update GESTION_DE_GATOS.Rol set descripcion = '" + textBox1.Text + "', estado=1 where idRol= " + ID;
                resultado = Home.BD.comando(consulta);
                resultado.Read();
                resultado.Close();
            }
            else
            {
                consulta = "update GESTION_DE_GATOS.Rol set descripcion = '" + textBox1.Text + "', estado=0 where idRol= " + ID;
                resultado = Home.BD.comando(consulta);
                resultado.Read();
                resultado.Close();
            }
            decimal idF = 0;
            if (!string.IsNullOrEmpty(comboBox2.Text) && (comboBox2.Text!="Ninguna"))
            {
                //agregar func, insert
                consulta = "select idFuncionalidad from GESTION_DE_GATOS.Funcionalidad where denominacion = '"+comboBox2.Text+"'";
                resultado = Home.BD.comando(consulta);
                if (resultado.Read())
                {
                    idF = resultado.GetDecimal(0);
                }
                resultado.Close();
                consulta = "EXEC GESTION_DE_GATOS.InsertarFuncXRol "+ ID + "," + idF;
                resultado = Home.BD.comando(consulta);
                decimal resu =0;
                if (resultado.Read())
                {
                    resu = resultado.GetDecimal(0);                    
                }
                if(resu!=0)
                {
                    MessageBox.Show("La funcionalidad fue agregada correctamente");
                }
                else
                {
                    MessageBox.Show("La funcionalidad no se pudo agregar correctamente");
                }
                resultado.Close();
            }
            if (!string.IsNullOrEmpty(comboBox3.Text) && (comboBox3.Text != "Ninguna"))
            {
                //delete
                consulta = "select idFuncionalidad from GESTION_DE_GATOS.Funcionalidad where denominacion = '"+comboBox3.Text+"'";
                resultado = Home.BD.comando(consulta);
                if (resultado.Read())
                {
                    idF = resultado.GetDecimal(0);
                }
                resultado.Close();
                consulta = "delete from GESTION_DE_GATOS.FuncXRol where funcionalidad = " + idF + " and rol = " + ID;
                resultado = Home.BD.comando(consulta);
                resultado.Read();
                resultado.Close();
                MessageBox.Show("La funcionalidad fue eliminada con exito");
            }
            MessageBox.Show("El rol ha sido modificado exitosamente");
            this.Close();


        }
    }
}
