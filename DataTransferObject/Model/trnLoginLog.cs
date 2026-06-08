using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataTransferObject.Model
{
    public class trnLoginLog
    {
        [Key]
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public DateTime LoginOn { get; set; }

    }
}
