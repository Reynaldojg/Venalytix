using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Domain.Base
{
    public abstract class APIBASE : AuditEntity
    {
        public int IdExterno { get; set; } //aqui vendra el ide de la api externa
        public string? Nombre { get; set; }
    }
}
