using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataTransferObject.Model
{
    public class HitCounter
    {
        [Key]
        public int HitCounterId { get; set; }

        public int IpAddressId { get; set; }
        [ForeignKey("IpAddressId")]
        public MIpAddress? MIpAddress { get; set; }

        public DateTime VisitDate { get; set; }
    }
}
