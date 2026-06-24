using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class TrnFwdCO
    {
        [Key]
        public int FwdCOId { get; set; }
        public int ApplicationId { get; set; }
        [ForeignKey(nameof(ApplicationId))]
        public CommonDataModel? CommonDataModel { get; set; }
        public int COUserId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

    }
}
