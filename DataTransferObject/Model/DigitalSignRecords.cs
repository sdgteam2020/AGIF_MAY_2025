using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class DigitalSignRecords
    {
        [Key]
        public int Id { get; set; }
        public string ArmyNo { get; set; }
        public string DomainId { get; set; }
        public string RankName { get; set; }
        public int ApplId { get; set; }
        public bool IsSign { get; set; }
        public bool IsRejectced { get; set; }
        public string XMLSignResponse { get; set; }
        public string Remarks { get; set; }
        public DateTime? SignOn { get; set; }
    }
}
