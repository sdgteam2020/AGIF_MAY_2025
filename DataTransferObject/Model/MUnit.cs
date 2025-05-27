using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class MUnit
    {
        [Key]
        public int UnitId { get; set; }
        public string Sus_no { get; set; }
        public string Suffix { get; set; }
        public string Abbreviation { get; set; }
        public bool IsVerify { get; set; }
        public bool IsActive { get; set; }
        public int Updatedby { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UnitName { get; set; }
    }
}
