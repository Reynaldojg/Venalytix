using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Interfaces.IBase
{
    public interface IRepositoryBase<TSave,TUpdate,TGet>
        where TSave :class
        where TUpdate : class
        where TGet : class
    {
        Task<OperationResult> AgregarAsync(TSave entidad);
        Task<OperationResult> ActualizarAsync(TUpdate entidad);
        Task<OperationResult> EliminarAsync(int id);
        Task<OperationResult> ObtenerPorAsync(int id);
        Task<OperationResult> ObtenerTodosAsync();
    }
}
