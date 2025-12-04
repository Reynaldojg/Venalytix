using Venalytix.Domain.OperationBase;

namespace Venalytix.Apication.Interfaces.ETL
{
    public interface ILoader
    {
        OperationResult Load(object data);
    }
}