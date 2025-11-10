using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venalytix.Apication.DTOS.DTOBase;

namespace Venalytix.Apication.DTOS.VentasDTO
{
    public class SaveVentaDTO :BaseDTO
    {
        public int IdCliente { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}
