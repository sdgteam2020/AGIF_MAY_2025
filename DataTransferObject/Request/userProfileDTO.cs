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

        [Required]
        public string userName { get; set; }

        [Required]
        [RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "Enter only alphabets and numbers")]
        [StringLength(11, ErrorMessage = "Please enter no more than 11 characters.")]
        public string ArmyNo { get; set; }

        [Required]
        public int rank { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }


        [Required]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Enter valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid mobile number.")]
        public string MobileNo { get; set; }

        [Required]
        public int regtCorps { get; set; }

        [Required(ErrorMessage = "Please Select Appointment.")]
        public int ApptId { get; set; }

        [Required]
        public int UnitId { get; set; }

    }
}
