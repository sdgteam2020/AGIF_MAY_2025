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
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
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
