using DataTransferObject.Identitytable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class UserMapping : Common
    {
        [Key]
        public int MappingId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? ApplicationUser { get; set; }

        [Required(ErrorMessage = "Profile ID is required.")]
        public int ProfileId { get; set; }

        [ForeignKey("ProfileId")]
        public UserProfile? UserProfile { get; set; }

        [Required(ErrorMessage = "Unit ID is required.")]
        public int UnitId { get; set; }

        [ForeignKey("UnitId")]
        public MUnit? MUnit { get; set; }
    }
}
