using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class UserProfile
    {
        [Key]
        public int ProfileId { get; set; }
        public string ArmyNo { get; set; } = string.Empty;
        public string userName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public int rank { get; set; }
        public int regtCorps { get; set; }
        public bool isActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ApptId { get; set; }
    }
}
