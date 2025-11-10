using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venalytix.Domain.Base;

namespace Venalytix.Domain.CSV
{
    public class Ventas : CSVBASE
    {
        public int IdVentas { get; set; }
        public int IdCliente { get; set; }
        public int IdProducto {get; set;}
        public int Cantidad {get; set;}
        public decimal Precio {get; set;}
        public DateTime Fecha { get; set;}

        //Propiedad calculada 
        public decimal Total => Cantidad * Precio;
    }
}
