using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Helpers
{
    public class SessionUserDTO
    {
        public string ArmyNo { get; set; } = string.Empty;
        public int ProfileId { get; set; }

        [StringLength(50, ErrorMessage = "UserName can't be longer than 50 characters.")]
        public string UserName { get; set; } = string.Empty;
        public bool IsCO { get; set; }
        public string Role { get; set; } = string.Empty;
        public int MappingId { get; set; }
        public int UserId { get; set; }
        public string? DomainId { get; set; }
        public string? RankName { get; set; }

        public string? ProfileName { get; set; }
        public string? AppointmentName { get; set; }


        [Required(ErrorMessage = "Mobile No is required.")]
        [StringLength(10, ErrorMessage = "Mobile No must be 10 digits.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid Mobile No.")]
        public string? MobileNo { get; set; }
        public string? RegtName { get; set; }
        public string? UnitName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(60, ErrorMessage = "Email cannot exceed 60 characters.")]
        public string? EmailId { get; set; }
        public bool IsPrimary { get; set; }
        public bool DteFmn { get; set; }
        public bool IsActive { get; set; }
        public bool IsCOActive { get; set; }
        public int RankId { get; set; }
        public int RegtId { get; set; }
        
        [Required(ErrorMessage = "Appointment is required.")]
        public int ApptId { get; set; }

        public int UnitId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name can't be longer than 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name must only contain alphabets and spaces.")]
        public string? name { get; set; } = string.Empty;
        public string? Nameid { get; set; }
        public string? AppName { get; set; }
    }
}
