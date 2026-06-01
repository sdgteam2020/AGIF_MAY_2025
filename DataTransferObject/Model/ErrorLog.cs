using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public int StatusCode { get; set; }
        public int ExceptionTypeId { get; set; }
        public string? Message { get; set; }
        public string? StackTrace { get; set; }
        public string? Path { get; set; }
        public DateTime? Created { get; set; } = DateTime.Now;
        [ForeignKey(nameof(ExceptionTypeId))]
        public virtual MExceptionType? MExceptionType { get; set; }
    }
}
