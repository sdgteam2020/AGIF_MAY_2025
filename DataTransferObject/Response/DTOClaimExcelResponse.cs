using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOClaimExcelResponse
    {
        public string? MaturityHolder_Name { get; set; }
        public string Rank { get; set; }
        public string Regt_Cps { get; set; }
        public string Unit { get; set; }
        public string? CDA_PAO { get; set; }
        public DateTime? Date_Of_Birth { get; set; }
        public DateTime? Enrollment_Date { get; set; }
        public DateTime? Retirement_Date { get; set; }
        public int? Year_Of_Service { get; set; }
        public int? Residual_Service { get; set; }
        public int ApplicationType { get; set; }

        public string? AadharNo { get; set; }
        public string? PANNo { get; set; }

        public string? Salary_Account_No { get; set; }
        public string? IFSC_Code { get; set; }

        public string? Bank_Branch { get; set; }

        public string? MobNo { get; set; }

        public string? E_Mail_Id { get; set; }

        public string ChldrenName { get; set; }
        public DateTime? ChildrenDOB { get; set; }

        public string? ChildbirthDOPartIIOrderNoAndDt { get; set; }

        public string Course { get; set; }
        public string NameofInstitute { get; set; }

        public DateTime? Marriagedt { get; set; }

        public string AgeOfWard { get; set; }
        public string AddressofProperty { get; set; }
        public string NameofPropertyHolder { get; set; }

        public string? SpecialReason { get; set; }

        public double Total_Cost { get; set; }

        public decimal? Amount_Applied_For_MAWD { get; set; }
        public string? NoofWithdrawal { get; set; }      

        public string? armyno{ get; set; }

        public string? Suffix { get; set; }

        public int? opfx { get; set; }

        public string? ono{ get; set; }

        public string? suffix_ { get; set; }

        public DateTime? dateandtimeofdocuuploadfrosanctioningauth{ get; set; }

        public string? IPaddress { get; set; }
        public string? EmailDomain { get; set; }

    }
}
