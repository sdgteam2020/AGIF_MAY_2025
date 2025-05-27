using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class MApplicantType
    {
        [Key]
        public int ApplicantTypeId { get; set; }
        public string ApplicantTypeName { get; set; } = string.Empty;
    }
}
