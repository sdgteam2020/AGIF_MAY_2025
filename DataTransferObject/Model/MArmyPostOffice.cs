using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class MArmyPostOffice
    {
        [Key]
        public int Id { get; set; }
        public string? ArmyPostOffice { get; set; } = string.Empty;
    }
}
