using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venalytix.Domain.OperationBase
{
    public class AuditEntity
    {
        public DateTime Fecha_creacion { get; set; } = DateTime.UtcNow;
        public string? Usuario_creo { get; set; }
        public DateTime? Fecha_actualizacion { get; set; }
        public string? Usuario_actualizacion { get; set; }
        public DateTime? Fecha_eliminacion { get; set; }
        public string? Usuario_eliminacion { get; set; }
        public bool Esta_eliminado { get; set; } = false;
    }
}
