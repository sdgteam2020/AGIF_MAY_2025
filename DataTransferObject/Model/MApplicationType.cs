using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class MApplicationType
    {
        [Key]
        public int ApplicationTypeId { get; set; }
        public string ApplicationTypeName { get; set; } = string.Empty;
        
    }
}
