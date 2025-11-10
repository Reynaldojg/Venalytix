using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venalytix.Domain.Base;

namespace Venalytix.Domain.CSV
{
    public class Clientes : CSVBASE
    {
        public int IdCLiente { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? Region { get; set; }
        public DateTime? FechaRegistro {  get; set; }
    }
}
