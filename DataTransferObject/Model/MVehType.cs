using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class MVehType
    {
        [Key]
        public int VehTypeId { get; set; }
        public string VehTypeName { get; set; } = string.Empty;

    }
}
