using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venalytix.Apication.DTOS.DTOBase;

namespace Venalytix.Apication.DTOS.ClientesDTO
{
    public class SaveClienteDTO:BaseDTO
    {
        public int IdCliente {  get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? Region { get; set; }
    }
}
