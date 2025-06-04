using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Request
{
    public class userProfileDTO
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "Enter only alphabets and numbers")]
        [StringLength(11, ErrorMessage = "Please enter no more than 11 characters.")]
        public string ArmyNo { get; set; }
        [Required]
        public string userName { get; set; }

        [Required]
        [RegularExpression("([a-zA-Z ])", ErrorMessage = "Enter only alphabets")]
        public string Name { get; set; }


        [Required]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Enter valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid mobile number.")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Please Select Appointment.")]
        public int ApptId { get; set; }

        public int regtCorps { get; set; }
        [Required]
        public int rank { get; set; }

        public bool isActive { get; set; }

        public string RankName { get; set; }

        public string UnitName { get; set; }
        public string UserId { get; set; }
        public int UnitId { get; set; }
        public string CreatedOn { get; set; }
        public int Type { get; set; }

    }
}
