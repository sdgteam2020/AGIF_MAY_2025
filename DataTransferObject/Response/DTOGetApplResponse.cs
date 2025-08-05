using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOGetApplResponse
    {
        public int ApplicationId { get; set; }
        public string ArmyNo { get; set; }
        public string OldArmyNo { get; set; }
        public string Name { get; set; }
        public string RegtCorps { get; set; }
        public string PresentUnit { get; set; }
        public string PcdaPao { get; set; }
        public string ApplicationType { get; set; }
        public string DateOfBirth { get; set; }
        public string AppliedDate { get; set; }
        public string PresentStatus { get; set; }
        public bool IsMergePdf { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DownloadedOn { get; set; }
        public int? DownloadCount { get; set; }
        public string? DigitalSignDate { get; set; }

    }
}
