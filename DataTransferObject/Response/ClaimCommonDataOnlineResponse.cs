using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class ClaimCommonDataOnlineResponse
    {
        public string ApplicationTypeName { get; set; }

        public int ApplicationType;

        public int ApplicationId { get; set; }


        public int ArmyPrefix { get; set; }


        public string? Number { get; set; }



        public string? Suffix { get; set; }


        public int OldArmyPrefix { get; set; }

        public string? OldNumber { get; set; }


        public string? OldSuffix { get; set; }



        public string DdlRank { get; set; }

        public string? ApplicantName { get; set; }


        public DateTime? DateOfBirth { get; set; }


        public DateTime? DateOfCommission { get; set; }


        public string? ExtnOfService { get; set; }

        public DateTime? DateOfPromotion { get; set; }


        public DateTime? DateOfRetirement { get; set; }


        public string? AadharCardNo { get; set; }


        public string? PanCardNo { get; set; }


        public string? MobileNo { get; set; }


        public string? Email { get; set; }


        public string? EmailDomain { get; set; }

        public int? TotalService { get; set; }

        public int? ResidualService { get; set; }


        public string RegtCorps { get; set; }


        public string? pcda_pao { get; set; }

        public string? pcda_AcctNo { get; set; }


        public string ParentUnit { get; set; }


        public string PresentUnit { get; set; }



        public string? PresentUnitPin { get; set; }

        public string ArmyPostOffice { get; set; }



        public string? CivilPostalAddress { get; set; }

        public string? NextFmnHQ { get; set; }


        public string? SalaryAcctNo { get; set; }


        public string? ConfirmSalaryAcctNo { get; set; }


        public string? IfsCode { get; set; }


        public string? NameOfBank { get; set; }


        public string? NameOfBankBranch { get; set; }

        public decimal? AmountwithdrwalRequired { get; set; }
        public string? NoOfwithdrwal { get; set; }


        public bool House_Building_Advance_Loan { get; set; }
        public bool House_Repair_Advance_Loan { get; set; }
        public bool Conveyance_Advance_Loan { get; set; }
        public bool Computer_Advance_Loan { get; set; }



        public DateTime? House_Building_Date_of_Loan_taken { get; set; }
        public int? House_Building_Duration_of_Loan { get; set; }
        public decimal? House_Building_Amount_Taken { get; set; }


        public DateTime? House_Repair_Advance_Date_of_Loan_taken { get; set; }
        public int? House_Repair_Advance_Duration_of_Loan { get; set; }
        public decimal? House_Repair_Advance_Amount_Taken { get; set; }


        public DateTime? Conveyance_Date_of_Loan_taken { get; set; }
        public int? Conveyance_Duration_of_Loan { get; set; }
        public decimal? Conveyance_Amount_Taken { get; set; }


        public DateTime? Computer_Date_of_Loan_taken { get; set; }
        public int? Computer_Duration_of_Loan { get; set; }
        public decimal? Computer_Amount_Taken { get; set; }




        public string? UpdatedOn { get; set; }
    }
}
