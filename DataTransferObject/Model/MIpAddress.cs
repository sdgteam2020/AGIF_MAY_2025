using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataTransferObject.Model
{
    public class MIpAddress
    {
        [Key]
        public int IpAddressId { get; set; }
        public string? IPAddress { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
    }
}
