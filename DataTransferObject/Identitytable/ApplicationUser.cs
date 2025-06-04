using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DataTransferObject.Identitytable
{
    public class ApplicationUser : IdentityUser<int>
    {
        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string DomainId { get; set; } = string.Empty;
        public bool Active { get; set; } = false;

        [Display(Name = "Updated By")]
        public int Updatedby { get; set; }

        [Display(Name = "Updated On")]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime UpdatedOn { get; set; }

        public int IntId { get; set; } = 0;

    }

}
