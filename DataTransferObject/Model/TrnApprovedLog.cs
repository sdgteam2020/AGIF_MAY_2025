using DataTransferObject.Identitytable;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Model
{
    public class TrnApprovedLog
    {
        [Key]
        public int ApprovedLogId { get; set; }

        public int AdminProfileId { get; set; }

        public string IpAddress { get; set; }

        public int CoProfileId { get; set; }
        [ForeignKey("CoProfileId")]
        public UserProfile? UserProfile { get; set; }
        public int AdminUserId { get; set; }

        public int CoUserId { get; set; }

        public bool IsApproved { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}