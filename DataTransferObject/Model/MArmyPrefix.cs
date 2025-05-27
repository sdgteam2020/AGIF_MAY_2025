using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class MArmyPrefix
    {
        [Key]
        public int Id { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public int UserType { get; set; }
    }
}
