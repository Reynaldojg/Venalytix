using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Interfaces.ETL
{
    public interface IExtractor
    {
        Task<OperationResult> ExtraerAsync();
    }
}
