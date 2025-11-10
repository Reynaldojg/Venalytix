using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Venalytix.Domain.API
{
    public class ApiFuente
    {
        public string? Nombre {  get; set; } // aqui se conseguira el nombre de la ApiExterna, ejemplos: "Api Externa de productos"
        public string? UrlBase { get; set; } // aqui se conseguira lo que seria URl de la api ejemplo: "https://api.ventasexternas.com/"
        public string? EndPoint { get; set; }
        public string? Metodo { get; set; }
        public DateTime UltimaACtualizacion {  get; set; }
    }
}
