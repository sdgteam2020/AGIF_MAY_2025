using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Helpers
{
    public static class Constants
    {
        public const string Appt = "1";
        public const string Unit = "2";
        public const string RankOffrs = "3";
        public const string RankJco = "4";
        public const string ApplicationType = "5";
        public const string ApplicantType = "6";
        public const string ArmyPrefix = "7";
        public const string RegtCorps = "8";
        public const string ArmyPrefixOfficers = "9";
        public const string ArmyPrefixJCO = "10";
        public const string ArmyPrefixOR = "11";
        public const string RetirementAge = "12";
        public const string RankOr = "13";
        public const string ArmyPostOffice = "14";
        public const string LoanFreq = "15";
        public const string LoanTypeHBA = "16";
        public const string LoanTypeCA = "17";
        public const string LoanTypePCA = "18";
        public const string PurposeOfWithdrawal = "19";
        public const string VehType = "20";


        #region Return To Front End

        public const int Success = 200;
        public const int BadRequest = 400;
        public const int InternalServerError = 500;
        public const int Save = 1;
        public const int Update = 2;
        public const int Exists = 3;
        public const int IncorrectData = 4;


        #endregion
    }
}
