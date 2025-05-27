using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class ErrorModel
    {
        public int StatusCode {  get; set; }
        public string? Message { get; set; }
        public string? ExceptionType { get; set; }
        public string? Details { get; set; }

        public ErrorModel(int statusCode,string? message,string? exceptionType,string? details = null) 
        { 
            StatusCode = statusCode;
            Message = message;
            Details = details;
            ExceptionType = exceptionType;
        }
    }
}
