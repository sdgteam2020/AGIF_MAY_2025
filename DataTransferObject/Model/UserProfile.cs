using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class UserProfile : Common
    {
        [Key]
        public int ProfileId { get; set; }

        [Required(ErrorMessage = "Army No is required.")]
        [RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "Enter only alphabets and numbers")]
        [StringLength(11, ErrorMessage = "Please enter no more than 11 characters.")]
        public string ArmyNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "User Name is required.")]
        public string userName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression("([a-zA-Z ])", ErrorMessage = "Enter only alphabets")]
        [StringLength(15, ErrorMessage = "Please enter no more than 15 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Enter valid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile No is required.")]
        [StringLength(10, ErrorMessage = "Mobile No must be 10 digits.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid Mobile No.")]
        public string MobileNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rank is required.")]
        public int rank { get; set; }

        [ForeignKey("rank")]
        public MRank? MRank { get; set; }

        [Required(ErrorMessage = "Regt/Corps is required.")]
        public int regtCorps { get; set; }

        [ForeignKey("regtCorps")]
        public MRegtCorps? MRegtCorps { get; set; }

        [Required(ErrorMessage = "ApptId is required.")]
        public int ApptId { get; set; }

        [ForeignKey("ApptId")]
        public MAppointment? MAppointments { get; set; }
    }
}
