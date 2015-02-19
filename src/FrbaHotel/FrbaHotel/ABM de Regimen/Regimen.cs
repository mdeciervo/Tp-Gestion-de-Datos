using System;

namespace FrbaHotel.ABM_de_Regimen
{
    public class Regimen
    {
        public Int64 codigo { get; set; }
        public string descripcion { get; set; }
        public Decimal precio { get; set; }
        public bool estado { get; set; }

        public Regimen(Int64 codigo, String descripcion, Decimal precio, bool estado)
        {
            this.codigo = codigo;
            this.descripcion = descripcion;
            this.precio = precio;
            this.estado = estado;
        }
    }
}
