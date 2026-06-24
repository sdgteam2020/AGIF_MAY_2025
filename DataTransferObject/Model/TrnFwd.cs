using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class TrnFwd
    {
        [Key]
        public int FwdId { get; set; }
        public int ApplicationId { get; set; }
        [ForeignKey(nameof(ApplicationId))]
        public CommonDataModel? CommonDataModel { get; set; }
        public int FromUserId { get; set; }
        public int FromProfileId { get; set; }
        public int ToUserId { get; set; }
        public int ToProfileId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
