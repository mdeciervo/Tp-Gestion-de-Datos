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
    public partial class Alta : Form
    {
        DataTable tabla;
        BindingSource bSource2;
        private SqlDataReader resultado;
        SqlDataAdapter sAdapter;
        DataTable dTable;
        string funciones = "";
        int cant = 0;
        public Alta()
        {
            InitializeComponent();
            
        }

        private void Alta_Load(object sender, EventArgs e)
        {
            string query = "select distinct F.idFuncionalidad ID,F.denominacion Denominacion from GESTION_DE_GATOS.Funcionalidad F" ;

            sAdapter = FrbaHotel.Home.BD.dameDataAdapter(query);
            dTable = FrbaHotel.Home.BD.dameDataTable(sAdapter);
            //BindingSource to sync DataTable and DataGridView
            BindingSource bSource = new BindingSource();
            //set the BindingSource DataSource
            bSource.DataSource = dTable;
            //set the DataGridView DataSource
            dataGridView1.DataSource = bSource;

            tabla = new DataTable();
            tabla.Columns.Add("Id");
            tabla.Columns.Add("Denominacion");
            bSource2 = new BindingSource();
            bSource2.DataSource = tabla ;
            //set the DataGridView DataSource
            dataGridView2.DataSource = bSource2;
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex==0)
            {
                
                string id = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                string funcion = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                int item = dataGridView1.CurrentRow.Index;
                dataGridView1.Rows.RemoveAt(item);
                DataRow row = tabla.NewRow();
                row["Id"] = id;
                row["Denominacion"] = funcion;
                tabla.Rows.Add(row);
                if (cant == 0)
                {
                    funciones = funciones + id.ToString();
                }
                else
                {
                    funciones = funciones + "," + id.ToString();
                }
                cant++;
                dataGridView2.DataSource = bSource2;
            }
           
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (funciones == "")
            {
                MessageBox.Show("Debe seleccionar al menos una funcion");
                return;
            }
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("El nombre tiene que estar completo");
                return;
            }
          
            decimal id = 0;
            string insert = "EXEC GESTION_DE_GATOS.InsertarRol '" + textBox1.Text + "',";
            if (checkBox1.Checked)
            {
                insert = insert + "1";
            }
            else
            {
                insert = insert + "0";
            }

            resultado = Home.BD.comando(insert);
            if (resultado.Read() == true)
            {
                id = resultado.GetDecimal(0);
            }
            resultado.Close(); 
            if(id==0)
            { 
               
                MessageBox.Show("No se pudo crear el rol porque ese nombre ya existe");
                button1_Click(null, null);
                return; 
            }

            string comand = "EXEC GESTION_DE_GATOS.InsertarFuncXRol "+id.ToString()+",";

            string[] strArr = null;
            int count = 0;
            char[] splitchar = { ',' };
            strArr = funciones.Split(splitchar);

            for (count = 0; count <= strArr.Length - 1; count++)
            {
                resultado = Home.BD.comando(comand + strArr[count]);
                if (!resultado.Read())
                {
                    
                    MessageBox.Show("No se puedo insertar la funcionalidad al rol");
                    return;
                }
                decimal a = resultado.GetDecimal(0);
                if (a==0)
                {
                    MessageBox.Show("No se puedo insertar la funcionalidad al rol");
                    return;
                }
                resultado.Close();
            }
            MessageBox.Show("Rol creado correctamente");
            this.Close();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox1.Focus();
            checkBox1.Checked = false;
            string query = "select distinct F.idFuncionalidad ID,F.denominacion Denominacion from GESTION_DE_GATOS.Funcionalidad F";
            sAdapter = FrbaHotel.Home.BD.dameDataAdapter(query);
            dTable = FrbaHotel.Home.BD.dameDataTable(sAdapter);
            //BindingSource to sync DataTable and DataGridView
            BindingSource bSource = new BindingSource();
            //set the BindingSource DataSource
            tabla.Clear();
            bSource.DataSource = dTable;
            //set the DataGridView DataSource
            dataGridView1.DataSource = bSource;
            dataGridView2.DataSource = bSource2;
            funciones = "";
            cant = 0;
    

        }
    }
}
