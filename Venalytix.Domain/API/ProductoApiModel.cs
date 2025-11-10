using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venalytix.Domain.Base;

namespace Venalytix.Domain.API
{
    public class ProductoApiModel : APIBASE
    {
        public string? Categoria { get; set; }
        public decimal Precio { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
