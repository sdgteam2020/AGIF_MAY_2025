using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public int StatusCode { get; set; }
        public string? ExceptionType { get; set; }
        public string? Message { get; set; }
        public string? StackTrace { get; set; }
        public string? Path { get; set; }
        public DateTime? Created { get; set; } = DateTime.Now;
    }
}
