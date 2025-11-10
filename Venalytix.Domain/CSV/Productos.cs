using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venalytix.Domain.Base;

namespace Venalytix.Domain.CSV
{
    public class Productos : CSVBASE
    {
      public int IdProductos { get; set; }
      public string? Nombre { get; set; }
      public string? Categoria { get; set; }
      public decimal Precio { get; set; }
      public bool Activo { get; set; } = true;
    }
}
