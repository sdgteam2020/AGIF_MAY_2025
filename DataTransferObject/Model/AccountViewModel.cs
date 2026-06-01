using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "UserName")]
        [RegularExpression(@"^[a-zA-Z0-9@._-]+$", ErrorMessage = "Invalid characters in Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(60, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 60 characters.")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Password must not contain spaces.")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public bool IsLockedOut { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public int FailedAttempts { get; set; }
        public int MaxAllowedAttempts { get; set; } = 5;
        public string LockoutMessage { get; set; } = String.Empty;
    }
}
