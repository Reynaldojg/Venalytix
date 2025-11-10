using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venalytix.Apication.DTOS.ProductosDTO;
using Venalytix.Apication.Interfaces.IBase;

namespace Venalytix.Apication.Interfaces.IRepositories
{
    public interface IProductoRepository : IRepositoryBase<SaveProductoDTO,UpdateProductoDTO,ObtenerProductoDTO>
    { 
    }
}
