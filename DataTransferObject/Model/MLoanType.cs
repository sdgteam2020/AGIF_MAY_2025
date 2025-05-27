using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class MLoanType
    {
        [Key]
        public int Id { get; set; }
        public string LoanType { get; set; } = string.Empty;
        public int LoanTypeCode { get; set; }
        public int ApplicationType { get; set; }
    }
}
