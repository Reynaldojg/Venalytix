using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venalytix.Apication.DTOS.DTOBase;

namespace Venalytix.Apication.DTOS.ProductosDTO
{
    public class ObtenerProductoDTO :BaseDTO
    {
        public int IdProducto { get; set; }
        public string? Nombre { get; set; }
        public string? Categoria { get; set; }
        public decimal Precio { get; set; }
    }
}
