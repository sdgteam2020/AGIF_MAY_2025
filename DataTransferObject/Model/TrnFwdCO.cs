using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string ArmyNo { get; set; }
        public string COUserId { get; set; }
        public int ProfileId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

    }
}
