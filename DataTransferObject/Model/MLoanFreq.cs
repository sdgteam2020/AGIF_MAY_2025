using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class MLoanFreq
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "LoanFreq is Required")]
        public int LoanFreq { get; set; }
    }
}
