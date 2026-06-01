using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataTransferObject.Model
{
    public class MExceptionType
    {
        [Key]
        public int ExceptionTypeId { get; set; }
        public string ? ExceptionTypeName { get; set; }

        public virtual ICollection<ErrorLog> ErrorLogs { get; set; }

    }
}
