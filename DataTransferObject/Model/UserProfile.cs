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
        public string ArmyNo { get; set; }
        public string userName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public int rank { get; set; }
        public int regtCorps { get; set; }
        public bool isActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ApptId { get; set; }
    }
}
