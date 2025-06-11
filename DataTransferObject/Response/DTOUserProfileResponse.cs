using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOUserProfileResponse
    {
        public string? DomainId { get; set; }
        public int MappingId { get; set; }
        public string? ProfileName { get; set; }
        public string? AppointmentName { get; set; }
        public string? ArmyNo { get; set; }
        public string? MobileNo { get; set; }
        public string? RankName { get; set; }
        public string? RegtName { get; set; }
        public string? UnitName { get; set; }
        public string? EmailId { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsFmn { get; set; }

    }
}
