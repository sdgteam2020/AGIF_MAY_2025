using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class UserMapping
    {
        [Key]
        public int MappingId { get; set; }
        public string UserID { get; set; }
        public int ProfileId { get; set; }
        public int UnitId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
