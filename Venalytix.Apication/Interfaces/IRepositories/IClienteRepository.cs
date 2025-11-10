using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venalytix.Apication.DTOS.ClientesDTO;
using Venalytix.Apication.Interfaces.IBase;

namespace Venalytix.Apication.Interfaces.IRepositories
{
    public interface IClienteRepository : IRepositoryBase<SaveClienteDTO,UpdateClienteDTO,ObtenerClienteDTO>
    {
    }
}
