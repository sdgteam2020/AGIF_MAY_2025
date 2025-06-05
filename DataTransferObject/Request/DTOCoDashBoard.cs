using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Request
{
    public class DTOCoDashBoard
    {
        public string EnApplication_Id { get; set; }
        public string ApplicationType { get; set; }
        public string Army_No { get; set; }
        public string Name { get; set; }
        public string Rank { get; set; }

        public DateTime? DateTimeUpdated { get; set; }

        public DateTime Date_Of_Birth { get; set; }
        public string DateTimeUpdatedString { get; set; }
        public bool IsCOApproved { get; set; }
        public bool IsCOReject { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public DateTime? RejectedTime { get; set; }
        public string RejectedApprovedRemarks { get; set; }
        public string ApplicationForm { get; set; }
        public bool IsMergePdf { get; set; }
        public int Application_Id { get; set; }
        public bool IsXMLDownload { get; set; }
        public string Date_Of_BirthString { get; set; }
        public string ApprovedTimeString { get; set; }
        public string RejectedTimeString { get; set; }
    }
}
