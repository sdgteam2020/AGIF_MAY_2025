using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOClaimExcelResponse
    {
        public string ApplicationTypeName { get; set; }

        public int ApplicationType;

        public int ApplicationId { get; set; }

        public string? ArmyNumber { get; set; }

        public string? OldArmyNumber { get; set; }

        public string Rank { get; set; }

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

        public string? Pers_Address_Line1 { get; set; }
        public string? Pers_Address_Line2 { get; set; }
        public string? Pers_Address_Line3 { get; set; }
        public string? Pers_Address_Line4 { get; set; }

        public string ChildName { get; set; }
        public DateTime? ChildDateOfBirth { get; set; }
        public string DOPartIINo { get; set; }
        public DateTime? DoPartIIDate { get; set; }
        public string AgeOfWard { get; set; }

        public DateTime? DateofMarriage { get; set; }

        public string CourseForWithdrawal { get; set; }
        public string CollegeInstitution { get; set; }
        public double TotalExpenditure { get; set; }

        public string AddressOfProperty { get; set; }
        public string PropertyHolderName { get; set; }

        public string? OtherReasons { get; set; }
        public int StatusCode { get; set; }


    }
}
