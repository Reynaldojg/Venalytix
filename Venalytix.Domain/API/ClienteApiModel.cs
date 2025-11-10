using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venalytix.Domain.Base;

namespace Venalytix.Domain.API
{
    public class ClienteApiModel : APIBASE
    {
        public string? Email {  get; set; }
        public string? Region { get; set; }
        public DataSetDateTime FechaActualizacion { get; set; }
    }
}
