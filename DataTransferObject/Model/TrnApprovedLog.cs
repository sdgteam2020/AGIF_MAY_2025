using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class TrnApprovedLog
    {
        [Key]
        public int ApprovedLogId {  get; set; }
        public string DomainId { get; set; }
        public string Name {  get; set; }
        public string IpAddress { get; set; }
        public string coDomainId { get; set; }

        public int coProfileId { get; set; }
        public bool IsApproved { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
