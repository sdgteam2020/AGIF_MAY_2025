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
        [Display(Name = "Updated On")]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime UpdatedOn { get; set; }
        [NotMapped]
        public int IntId { get; set; }
    }

}
