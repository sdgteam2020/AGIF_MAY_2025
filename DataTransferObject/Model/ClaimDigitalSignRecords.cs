using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class ClaimDigitalSignRecords
    {
        [Key]
        public int Id { get; set; }
        public string ArmyNo { get; set; } = string.Empty;
        public string DomainId { get; set; } = string.Empty;
        public string RankName { get; set; } = string.Empty;
        public int ApplId { get; set; }
        public bool IsSign { get; set; }
        public bool IsRejectced { get; set; }
        public string XMLSignResponse { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public DateTime? SignOn { get; set; }
    }
}
