using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Required(ErrorMessage = "LoanTypeCode is Required")]
        public int LoanTypeCode { get; set; }

        [Required(ErrorMessage = "ApplicationType is Required")]
        public int ApplicationType { get; set; }
        [ForeignKey("ApplicationType")]
        public MApplicationType? MApplicationTypes { get; set; }
    }
}
