using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTODigitalSignDataResponse
    {
        public int ApplicationId { get; set; }
        public string ApplicantName { get; set; }
        public string ArmyNo { get; set; }
        public bool IsRejectced { get; set; }
        public int ApplicationType { get; set; }
        public string RankName { get; set; }
        public int Unit { get; set; }
        public string PCDA_PAO { get; set; }
        public string Date_Of_Birth { get; set; }
        public string Retirement_Date { get; set; }
        public string CDA_Account_No { get; set; }
        public string Mobile_No { get; set; }
        public int ApplType { get; set; }
        public string PAN_No { get; set; }
        public string DateOfCommision { get; set; }
        public string UnitName { get; set; }
        public string AccountNo { get; set; }
    }
}
