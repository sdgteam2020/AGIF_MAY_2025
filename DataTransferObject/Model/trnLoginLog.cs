using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataTransferObject.Model
{
    public class trnLoginLog
    {
        [Key]
        public int Id { get; set; }
        public int ProfileId { get; set; }
        [ForeignKey("ProfileId")]
        public UserProfile? UserProfile { get; set; }
        public int IpAddressId { get; set; }
        [ForeignKey("IpAddressId")]
        public MIpAddress? MIpAddress { get; set; }
        public DateTime LoginOn { get; set; }

    }
}
