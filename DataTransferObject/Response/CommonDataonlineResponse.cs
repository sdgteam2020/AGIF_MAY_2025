using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class CommonDataonlineResponse
    {
        public int ApplicationType;
        public string ApplicationTypeName { get; set; }
        public string ApplicationTypeAbbr { get; set; }

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

        public string? CoName {get; set; }

        public string? UpdatedOn { get; set; }
    }
}
