using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Interfaces.IExtractor
{
    public interface IExtractor
    {
        Task<OperationResult> ExtraerAsync();
    }
}
