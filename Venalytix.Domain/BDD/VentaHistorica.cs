using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Domain.BDD
{
    public class VentaHistorica : AuditEntity
    {
        public int Id { get; set; }
        public int IdCliente {get; set;}
        public int IdProducto { get; set;}
        public int Cantidad { get; set;}
        public decimal Precio { get; set;}
        public DateTime Fecha { get; set;}
    }
}
