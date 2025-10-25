using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inspire.Erp.Domain.Modals
{
    public class Response<T>
    {
        public T Result { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public bool Valid { get; set; }
        public int TotalCount { get; set; }
        public decimal TotalValue { get; set; }
        public static Response<T> Success(T data, string message, int totalCout = 0, decimal totalValue = 0m)
        {
            return new Response<T>()
            {
                Valid = true,
                Message = message,
                Result = data,
                TotalCount = totalCout,
                TotalValue = totalValue,
                Status = "Success"
            };
        }
        public static Response<T> Fail(T data, string message)
        {
            return new Response<T>()
            {
                Valid = false,
                Message = message,
                Result = data,
                Status = "Fail"
            };
        }


    }

}
