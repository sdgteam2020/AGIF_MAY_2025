using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOExcelResponse
    {
        public string Loanee_Name { get; set; }
        public string Rank { get; set; }
        public string Regt_Corps { get; set; }
        public string Unit { get; set; }
        public string CDA_PAO { get; set; }
        public DateTime? Date_Of_Birth { get; set; }
        public DateTime? Enrollment_Date { get; set; }
        public DateTime? Promotion_Date { get; set; }
        public DateTime? Retirement_Date { get; set; }
        public int? Year_Of_Service { get; set; }
        public int? Residual_Service { get; set; }
        public int ApplicationType { get; set; }
        public string LoanType { get; set; }//?????????????????
        public string AadharNo { get; set; }
        public string PANNo { get; set; }
        public string Salary_Slip_Month_Year { get; set; }
        public string CDA_Account_No { get; set; }

        public decimal? Basic_Salary { get; set; }
        public decimal? Rank_Grade_Pay { get; set; }
        public decimal? MSP { get; set; }
        public decimal? NPA_X_Pay { get; set; }
        public decimal? Tech_Pay { get; set; }
        public decimal? DA { get; set; }
        public decimal? TPTL_Pay { get; set; }//??????????????
        public decimal? MISC_Pay { get; set; }
        public decimal? PLI { get; set; }
        public decimal? Rev_IT { get; set; }//??????????????
        public decimal? AGIF { get; set; }
        public decimal? Income_Tax_Monthly { get; set; }
        public decimal? DSOP_AFPP { get; set; }
        public decimal? MISC { get; set; }

        public decimal? Total { get; set; }
        public decimal? Salary_After_Deduction { get; set; }

        public string Dealer_Name { get; set; }
        public string Vehicle_Name { get; set; }
        public string Vehicle_Make { get; set; }
        public decimal? Total_Cost { get; set; }
        public decimal? Amount_Applied_For_Loan { get; set; }
        public decimal? No_Of_EMI_Applied { get; set; }
        public string Pers_Address_Line1 { get; set; }
        public string Pers_Address_Line2 { get; set; }
        public string Pers_Address_Line3 { get; set; }
        public string Pers_Address_Line4 { get; set; }
        public string Pin_Code { get; set; }
        public string Payee_Account_No { get; set; }
        public string IFSC_Code { get; set; }
        public string Mobile_No { get; set; }
        public string E_Mail_Id { get; set; }
        public string Previous_Loan_Purpoes { get; set; }//??????????????
        public string Amount { get; set; } //??????????????
        public string EMI { get; set; } //??????????????
        public string Previous_Loan_Is_Paid { get; set; } //??????????????

        public string apfx { get; set; }
        public string ano { get; set; }
        public string asfx { get; set; }
        public string opfx { get; set; }
        public string ono { get; set; }
        public string osfx { get; set; }

        //New fields


        public int ApplicationID { get; set; }
        public decimal? CL_Pay { get; set; }
        public decimal? EducationCess { get; set; }
        public decimal? LoanEMI_Outside { get; set; }
        public decimal? LoanEMI_AGIF { get; set; }
        public decimal? LRA { get; set; }
        public decimal? PMHA { get; set; }
        public string ParentUnit { get; set; }
        public int VehType { get; set; }
        public int StatusCode { get; set; }
        public string Remarks { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string COEmailId { get; set; }
        public string MobileNo { get; set; }

    }

}
