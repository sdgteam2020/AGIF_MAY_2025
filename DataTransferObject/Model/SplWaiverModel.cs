using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class SplWaiverModel : Common
    {

        [Key]
        public int SWId { get; set; }

        [Required(ErrorMessage = "Application ID is required.")]
        public int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public ClaimCommonModel? ClaimCommonModel { get; set; }

        [Required]
        public string? OtherReasons { get; set; }
    }
}
