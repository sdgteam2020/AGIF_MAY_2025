using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOExcelResponse
    {

        public string? ApplicantName { get; set; }
        public string Rank { get; set; }//change in name
        public string RegtCorps { get; set; }
        public string PresentUnit { get; set; }
        public string ParentUnit { get; set; }
        public string? pcda_pao { get; set; }
        public string? pcda_AcctNo { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfCommission { get; set; }
        public DateTime? DateOfPromotion { get; set; }
        public DateTime? DateOfRetirement { get; set; }
        public int? TotalService { get; set; }
        public int? ResidualService { get; set; }
        public string ApplicationTypeAbbr { get; set; }






        public int ApplicationType;
        public string ApplicationTypeName { get; set; }
        

        public int ApplicationId { get; set; }


        public int ArmyPrefix { get; set; }


        public string? Number { get; set; }



        public string? Suffix { get; set; }


        public int OldArmyPrefix { get; set; }

        public string? OldNumber { get; set; }


        public string? OldSuffix { get; set; }



        



        


       


        public string? ExtnOfService { get; set; }

        


        

        public string? AadharCardNo { get; set; }


        public string? PanCardNo { get; set; }


        public string? MobileNo { get; set; }


        public string? Email { get; set; }


        public string? EmailDomain { get; set; }

        

       


        


        

        

       



        public string? PresentUnitPin { get; set; }

        public string ArmyPostOffice { get; set; }



        public string? CivilPostalAddress { get; set; }

        public string? NextFmnHQ { get; set; }

        public string? Vill_Town { get; set; }


        public string? PostOffice { get; set; }


        public string? Distt { get; set; }



        public string? State { get; set; }


        public string? Code { get; set; }


        public string? SalaryAcctNo { get; set; }


        public string? ConfirmSalaryAcctNo { get; set; }


        public string? IfsCode { get; set; }


        public string? NameOfBank { get; set; }


        public string? NameOfBankBranch { get; set; }

        // Financial information



        public DateTime? MonthlyPaySlip { get; set; }


        public decimal? BasicPay { get; set; }

        public decimal? dsop_afpp { get; set; }

        public decimal? rank_gradePay { get; set; }


        public decimal? agif_Subs { get; set; }


        public decimal? Msp { get; set; }

        public decimal? IncomeTaxMonthly { get; set; }

        public decimal? CI_Pay { get; set; }


        public decimal? EducationCess { get; set; }


        public decimal? npax_Pay { get; set; }


        public decimal? Pli { get; set; }

        public string? TechPay { get; set; }


        public decimal? misc_Deduction { get; set; }



        public decimal? Da { get; set; }

        public decimal? loanEMI_Outside { get; set; }


        public decimal? Pmha { get; set; }


        public decimal? LoanEmi { get; set; }


        public decimal? Lra { get; set; }

        public decimal? MiscPay { get; set; }


        public decimal? TotalCredit { get; set; }

        public decimal? TotalDeductions { get; set; }


        public decimal? salary_After_Deductions { get; set; }

        public string? CoName { get; set; }

        /////////////////////////////// CAR////////////////

        public string? DealerName { get; set; }
        public string Veh_Loan_Type { get; set; }
        public string? CompanyName { get; set; }
        public string? ModelName { get; set; }
        public int? VehicleCost { get; set; }
        public int? CA_LoanFreq { get; set; }
        public decimal? CA_Amt_Eligible_for_loan { get; set; }
        public decimal? CA_EMI_Eligible { get; set; }
        public decimal? CA_repayingCapacity { get; set; }
        public decimal? CA_Amount_Applied_For_Loan { get; set; }
        public decimal? CA_EMI_Applied { get; set; }
        public decimal? CA_approxEMIAmount { get; set; }
        public string? DrivingLicenseNo { get; set; }
        public string? Validity_Date_DL { get; set; }
        public string? DL_IssuingAuth { get; set; }
        public decimal? CA_approxDisbursementAmt { get; set; }

        /////////////////////////////// HBA////////////////
        public string PropertyType { get; set; }


        public string? PropertySeller { get; set; }


        public string? PropertyAddress { get; set; }


        public decimal? PropertyCost { get; set; }


        public int? HBA_LoanFreq { get; set; }


        public decimal? HBA_repayingCapacity { get; set; }

        public decimal? HBA_Amt_Eligible_for_loan { get; set; }


        public decimal? HBA_EMI_Eligible { get; set; }


        public decimal? HBA_Amount_Applied_For_Loan { get; set; }


        public decimal? HBA_EMI_Applied { get; set; }


        public decimal? HBA_approxEMIAmount { get; set; }


        public decimal? HBA_approxDisbursementAmt { get; set; }

        /////////////////////////////// PCA ////////////////
        public string? PCA_dealerName { get; set; }
        public string computer_Loan_Type { get; set; }
        public string? PCA_companyName { get; set; }
        public string? PCA_modelName { get; set; }

        public decimal? computerCost { get; set; }
        public int? PCA_LoanFreq { get; set; }

        public decimal? PCA_Amt_Eligible_for_loan { get; set; }
        public decimal? PCA_EMI_Eligible { get; set; }
        public decimal? PCA_repayingCapacity { get; set; }

        public decimal? PCA_Amount_Applied_For_Loan { get; set; }

        public decimal? PCA_EMI_Applied { get; set; }

        public decimal? PCA_approxEMIAmount { get; set; }
        public decimal? PCA_approxDisbursementAmt { get; set; }
    }
}
