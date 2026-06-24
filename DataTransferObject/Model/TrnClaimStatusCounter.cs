using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class TrnClaimStatusCounter
    {
        [Key]
        public int Id { get; set; }

        public int StatusId { get; set; }

        public int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public ClaimCommonModel? ClaimCommonModel { get; set; }

        public DateTime? ActionOn { get; set; }
    }
}
