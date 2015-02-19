using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace FrbaHotel.Regimen
{
    public partial class Listado : Form
    {
        SqlDataAdapter sAdapter;
        DataTable dTable;
        string accionString;
        string query;

        public Listado()
        {
            InitializeComponent();
        }

        public Listado(string accion, string queryParam)
        {
            InitializeComponent();
            accionString = accion;
            query = queryParam;
        }

        private void buttonLimpiar_Click(object sender, EventArgs e)
        {
            textBoxCodigo.Text = String.Empty;
            textBoxDescripcion.Text = String.Empty;
            textBoxPrecio.Text = String.Empty;
            checkBoxExtado.Checked = false;
            this.buttonBuscar_Click(null,null);
        }

        private void Listado_Load(object sender, EventArgs e)
        {

            string query = "SELECT * FROM GESTION_DE_GATOS.Regimen";
            sAdapter = FrbaHotel.Home.BD.dameDataAdapter(query);
            dTable = FrbaHotel.Home.BD.dameDataTable(sAdapter);
            //BindingSource to sync DataTable and DataGridView
            BindingSource bSource = new BindingSource();
            //set the BindingSource DataSource
            bSource.DataSource = dTable;
            //set the DataGridView DataSource
            dataGridView1.DataSource = bSource;
        }

        private void buttonBuscar_Click(object sender, EventArgs e)
        {
            DataView dvData = new DataView(dTable);
            string query = "";
            query = query + this.filtrarAproximadamentePor("descripcion", textBoxDescripcion.Text);
            query = query + this.filtrarExactamentePor("codigo",textBoxCodigo.Text);
            query = query + this.filtrarExactamentePor("precio", textBoxPrecio.Text.Replace(',', '.'));
            if (query.Length > 0) { query = query.Remove(query.Length - 4); }
            dvData.RowFilter = query;
            dataGridView1.DataSource = dvData;
        }

        private string filtrarExactamentePor(string columna, string valor){
            if (!string.IsNullOrEmpty(valor))
            {
                return columna + " = '" + valor + "' AND ";
            }
            return "";
        }

        private string filtrarAproximadamentePor(string columna, string valor)
        {
            if (!string.IsNullOrEmpty(valor))
            {
                return columna + " LIKE '%" + valor + "%' AND ";
            }
            return "";
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                Int64 codigo = Convert.ToInt64(dataGridView1.CurrentRow.Cells[1].Value);
                string descripcion = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                Decimal precio = Convert.ToDecimal(dataGridView1.CurrentRow.Cells[3].Value);
                bool estado = dataGridView1.CurrentRow.Cells[4].Value.Equals(true);
                ABM_de_Regimen.Regimen regimen = new ABM_de_Regimen.Regimen(codigo, descripcion, precio, estado);
                Regimen.Modificacion modificar = new Regimen.Modificacion(regimen,accionString,query);
                this.Hide();
                modificar.ShowDialog();
                this.Listado_Load(null,null);
                this.Show();
            }
        }

        }
}

