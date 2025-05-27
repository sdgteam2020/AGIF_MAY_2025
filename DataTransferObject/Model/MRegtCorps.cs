using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class MRegtCorps
    {
        [Key]
        public int Id { get; set; }
        public string RegtName { get; set; } = string.Empty;
        public string PCDA_PAO { get; set; } = string.Empty;
    }
}
