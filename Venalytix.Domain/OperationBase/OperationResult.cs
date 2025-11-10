using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venalytix.Domain.OperationBase
{
    public class OperationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string .Empty;
        public dynamic? Data { get; set; }

        public static OperationResult Success(dynamic? data = null, string message = "Operacion exitosa")
        {
            return new OperationResult
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }
        public static OperationResult Failure(string message = "Ocurrio un error.")
        {
            return new OperationResult
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
}
