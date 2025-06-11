using DataTransferObject.Identitytable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class Common
    {
        [Required]
        public bool IsActive { get; set; } = true;

        public int? Updatedby { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime2")]
        public DateTime? UpdatedOn { get; set; } 
    }
}
