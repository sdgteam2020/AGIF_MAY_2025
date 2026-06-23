using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using DataTransferObject.Response;
using DataTransferObject.Request;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DataAccessLayer.Repositories
{
    public class ModelValidationServiceDL : IModelValidationService
    {
        private readonly IMasterOnlyTable _IMasterOnlyTable;
        public ModelValidationServiceDL(IMasterOnlyTable MasterOnlyTable)
        {
            _IMasterOnlyTable = MasterOnlyTable;
        }

        // HBA Validation Methods
        public async Task ValidateHBADetails(DTOOnlineApplication model, ModelStateDictionary modelState)
        {
            if (model.HBAApplication == null ||
                model.CommonData == null)
            {
                return;
            }

            var hba = model.HBAApplication;

            decimal repayingCapacity =
                CalculateHBARepayingCapacity(model);

            decimal eligibleLoan =
                CalculateHBAEligibleLoan(model);

            decimal eligibleEmi =
                CalculateHBAEligibleEMI(model, (DateTime)model.CommonData.DateOfRetirement);

            decimal disbursement =
                CalculateHBADisbursement(model);

            decimal emiAmount =
                CalculateHBAEMI(model);

            if ((hba.HBA_repayingCapacity ?? 0) != repayingCapacity)
            {
                modelState.AddModelError(
                    "HBAApplication.HBA_repayingCapacity",
                    "Invalid repaying capacity.");
            }

            if ((hba.HBA_Amt_Eligible_for_loan ?? 0) != eligibleLoan)
            {
                modelState.AddModelError(
                    "HBAApplication.HBA_Amt_Eligible_for_loan",
                    "Invalid eligible loan amount.");
            }

            if ((hba.HBA_EMI_Eligible ?? 0) != eligibleEmi)
            {
                modelState.AddModelError(
                    "HBAApplication.HBA_EMI_Eligible",
                    "Invalid EMI eligibility.");
            }

            if ((hba.HBA_approxDisbursementAmt ?? 0) != disbursement)
            {
                modelState.AddModelError(
                    "HBAApplication.HBA_approxDisbursementAmt",
                    "Invalid disbursement amount.");
            }

            if (Math.Abs((hba.HBA_approxEMIAmount ?? 0) - emiAmount) > 1)
            {
                modelState.AddModelError(
                    "HBAApplication.HBA_approxEMIAmount",
                    "Invalid EMI amount.");
            }

            if ((hba.HBA_Amount_Applied_For_Loan ?? 0) > eligibleLoan)
            {
                modelState.AddModelError(
                    "HBAApplication.HBA_Amount_Applied_For_Loan",
                    "Loan amount exceeds eligibility.");
            }

            if ((hba.HBA_EMI_Applied ?? 0) > eligibleEmi)
            {
                modelState.AddModelError(
                    "HBAApplication.HBA_EMI_Applied",
                    "EMI exceeds eligibility.");
            }

            if (emiAmount > repayingCapacity)
            {
                modelState.AddModelError(
                    "HBAApplication.HBA_approxEMIAmount",
                    "EMI exceeds repayment capacity.");
            }

            await Task.CompletedTask;
        }

        private decimal CalculateHBARepayingCapacity(DTOOnlineApplication model)
        {
            decimal credit =
                model.CommonData.TotalCredit ?? 0;

            decimal debit =
                model.CommonData.TotalDeductions ?? 0;

            decimal capacity =
                (credit * 0.75m) - debit;

            return Math.Max(0, capacity);
        }
        private decimal CalculateHBAEligibleLoan(DTOOnlineApplication model)
        {
            decimal propertyCost =
                model.HBAApplication.PropertyCost ?? 0;

            int propertyType =
                model.HBAApplication.PropertyType;

            int prefix =
                model.CommonData.ArmyPrefix;

            propertyCost =
                Math.Round(propertyCost * 0.85m);

            decimal maxAmount;

            if (propertyType == 5)
            {
                maxAmount = 2000000;
            }
            else
            {
                if (prefix == 13)
                {
                    maxAmount = 5000000;
                }
                else if (prefix == 14)
                {
                    maxAmount = 4000000;
                }
                else
                {
                    maxAmount = 10000000;
                }
            }

            return Math.Min(propertyCost, maxAmount);
        }
        private decimal CalculateHBAEligibleEMI(DTOOnlineApplication model, DateTime retirementDate)
        {
            int residualMonths = CalculateResidualMonth(retirementDate);
            residualMonths -= 6;
            int emiLimit = model.HBAApplication.PropertyType == 5 ? 120 : 240;

            return Math.Min(emiLimit, residualMonths);
        }
        private decimal CalculateHBADisbursement(DTOOnlineApplication model)
        {
            return Math.Round(
                (model.HBAApplication.HBA_Amount_Applied_For_Loan ?? 0)
                * 0.99m,
                0);
        }
        private decimal CalculateHBAEMI(DTOOnlineApplication model)
        {
            int propertyType = model.HBAApplication.PropertyType;
            string applicantType = model.applicantCategory;

            decimal principal = model.HBAApplication.HBA_Amount_Applied_For_Loan ?? 0m;
            int months = Convert.ToInt32(model.HBAApplication.HBA_EMI_Applied ?? 0);

            decimal yearlyRate = 0m;

            if (propertyType == 5)
            {
                yearlyRate = (applicantType == "1") ? 8.00m : 7.50m;
            }
            else
            {
                yearlyRate = (applicantType == "1") ? 7.50m : 7.00m;
            }

            decimal monthlyRate = (yearlyRate / 12m) / 100m;

            if (principal <= 0 || months <= 0 || monthlyRate <= 0)
            {
                return 0;
            }

            double p = (double)principal;
            double r = (double)monthlyRate;
            double n = months;

            double emi = (p * r * Math.Pow(1 + r, n)) / (Math.Pow(1 + r, n) - 1);

            return Math.Round((decimal)emi, 0);
        }

        // CA Validation Methods
        public async Task ValidateCADetails(DTOOnlineApplication model, ModelStateDictionary modelState)
        {
            var ca = model.CarApplication;

            if (ca == null)
                return;

            decimal repayingCapacity =
                CalculateCARepayingCapacity(model);

            decimal eligibleLoan =
                CalculateCAEligibleLoan(model);

            decimal eligibleEmi =
                CalculateCAEligibleEMI(model, (DateTime)model.CommonData.DateOfRetirement);

            decimal disbursement =
                CalculateCADisbursement(model);

            decimal emiAmount =
                CalculateCAEMI(model);
            if ((ca.CA_repayingCapacity ?? 0) != repayingCapacity)
            {
                modelState.AddModelError(
                    "CarApplication.CA_repayingCapacity",
                    "Invalid repaying capacity.");
            }

            if ((ca.CA_Amt_Eligible_for_loan ?? 0) != eligibleLoan)
            {
                modelState.AddModelError(
                    "CarApplication.CA_Amt_Eligible_for_loan",
                    "Invalid eligible loan amount.");
            }

            if ((ca.CA_EMI_Eligible ?? 0) != eligibleEmi)
            {
                modelState.AddModelError(
                    "CarApplication.CA_EMI_Eligible",
                    "Invalid EMI eligibility.");
            }

            if ((ca.CA_approxDisbursementAmt ?? 0) != disbursement)
            {
                modelState.AddModelError(
                    "CarApplication.CA_approxDisbursementAmt",
                    "Invalid disbursement amount.");
            }

            if (Math.Abs((ca.CA_approxEMIAmount ?? 0) - emiAmount) > 1)
            {
                modelState.AddModelError(
                    "CarApplication.CA_approxEMIAmount",
                    "Invalid EMI amount.");
            }

            if ((ca.CA_Amount_Applied_For_Loan ?? 0) > eligibleLoan)
            {
                modelState.AddModelError(
                    "CarApplication.CA_Amount_Applied_For_Loan",
                    "Loan amount exceeds eligibility.");
            }

            if ((ca.CA_EMI_Applied ?? 0) > eligibleEmi)
            {
                modelState.AddModelError(
                    "CarApplication.CA_EMI_Applied",
                    "EMI exceeds eligibility.");
            }

            if (emiAmount > repayingCapacity)
            {
                modelState.AddModelError(
                    "CarApplication.CA_approxEMIAmount",
                    "EMI exceeds repayment capacity.");
            }

            await Task.CompletedTask;

        }
        private decimal CalculateCARepayingCapacity(DTOOnlineApplication model)
        {
            decimal credit =
                model.CommonData.TotalCredit ?? 0;

            decimal debit =
                model.CommonData.TotalDeductions ?? 0;

            return Math.Max(
                0,
                (credit * 0.75m) - debit);
        }
        private decimal CalculateCAEligibleLoan(DTOOnlineApplication model)
        {
            var ca = model.CarApplication;

            decimal vehicleCost =
                Math.Round((ca.VehicleCost ?? 0) * 0.90m);

            int prefix =
                model.CommonData.ArmyPrefix;

            decimal maxAmount = 0;

            if (ca.Veh_Loan_Type == 2)
            {
                if (ca.VehTypeId == 4)
                {
                    maxAmount =
                        (prefix == 13 || prefix == 14)
                            ? 1500000
                            : 2500000;
                }
                else
                {
                    maxAmount =
                        (prefix == 13 || prefix == 14)
                            ? 1000000
                            : 2000000;
                }
            }
            else if (ca.Veh_Loan_Type == 3)
            {
                maxAmount =
                    (prefix == 13 || prefix == 14)
                        ? 500000
                        : 1000000;
            }
            else
            {
                maxAmount =
                    (prefix == 13 || prefix == 14)
                        ? 200000
                        : 1000000;
            }

            return Math.Min(
                vehicleCost,
                maxAmount);
        }
        private decimal CalculateCAEligibleEMI(DTOOnlineApplication model, DateTime retirementDate)
        {
            var ca = model.CarApplication;

            int residualMonths = CalculateResidualMonth(retirementDate);
            residualMonths -= 6;
            int emi = 60;

            if (ca.Veh_Loan_Type == 2)
            {
                emi =
                    ca.CA_LoanFreq == 2
                        ? 72
                        : 96;
            }
            else if (ca.Veh_Loan_Type == 3)
            {
                emi = 72;
            }

            return Math.Min(
                emi,
                residualMonths);
        }
        private decimal CalculateCADisbursement(DTOOnlineApplication model)
        {
            return Math.Round(
                (model.CarApplication.CA_Amount_Applied_For_Loan ?? 0)
                * 0.99m,
                0);
        }
        private decimal CalculateCAEMI(DTOOnlineApplication model)
        {
            decimal principal =
                model.CarApplication.CA_Amount_Applied_For_Loan ?? 0;

            int months =
                Convert.ToInt32(
                    model.CarApplication.CA_EMI_Applied ?? 0);

            decimal monthlyRate =
                (8.25m / 12m) / 100m;

            if (principal <= 0 || months <= 0)
            {
                return 0;
            }

            double p = (double)principal;
            double r = (double)monthlyRate;
            double n = months;

            double emi =
                (p * r * Math.Pow(1 + r, n))
                / (Math.Pow(1 + r, n) - 1);

            return Math.Round((decimal)emi, 0);
        }

        // PCA Validation Methods
        public async Task ValidatePCADetails(DTOOnlineApplication model, ModelStateDictionary modelState)
        {
            var pca = model.PCAApplication;

            if (pca == null)
                return;

            decimal repayingCapacity =
                CalculatePCARepayingCapacity(model);

            decimal eligibleLoan =
                CalculatePCAEligibleLoan(model);

            decimal eligibleEmi =
                CalculatePCAEligibleEMI(model, (DateTime)model.CommonData.DateOfRetirement);

            decimal emiAmount =
                CalculatePCAEMI(model);

            decimal disbursement =
                CalculatePCADisbursement(model);

            if ((pca.PCA_repayingCapacity ?? 0) != repayingCapacity)
            {
                modelState.AddModelError(
                    "PCAApplication.PCA_repayingCapacity",
                    "Invalid repaying capacity.");
            }

            if ((pca.PCA_Amt_Eligible_for_loan ?? 0) != eligibleLoan)
            {
                modelState.AddModelError(
                    "PCAApplication.PCA_Amt_Eligible_for_loan",
                    "Invalid eligible loan amount.");
            }

            if ((pca.PCA_EMI_Eligible ?? 0) != eligibleEmi)
            {
                modelState.AddModelError(
                    "PCAApplication.PCA_EMI_Eligible",
                    "Invalid EMI eligibility.");
            }

            if ((pca.PCA_approxDisbursementAmt ?? 0) != disbursement)
            {
                modelState.AddModelError(
                    "PCAApplication.PCA_approxDisbursementAmt",
                    "Invalid disbursement amount.");
            }

            if (Math.Abs((pca.PCA_approxEMIAmount ?? 0) - emiAmount) > 1)
            {
                modelState.AddModelError(
                    "PCAApplication.PCA_approxEMIAmount",
                    "Invalid EMI amount.");
            }

            if ((pca.PCA_Amount_Applied_For_Loan ?? 0) > eligibleLoan)
            {
                modelState.AddModelError(
                    "PCAApplication.PCA_Amount_Applied_For_Loan",
                    "Loan amount exceeds eligibility.");
            }

            if ((pca.PCA_EMI_Applied ?? 0) > eligibleEmi)
            {
                modelState.AddModelError(
                    "PCAApplication.PCA_EMI_Applied",
                    "EMI exceeds eligibility.");
            }

            if (emiAmount > repayingCapacity)
            {
                modelState.AddModelError(
                    "PCAApplication.PCA_approxEMIAmount",
                    "EMI exceeds repayment capacity.");
            }

            await Task.CompletedTask;
        }
        private decimal CalculatePCAEligibleLoan(DTOOnlineApplication model)
        {
            decimal computerCost =
                Math.Round(
                    (model.PCAApplication.computerCost ?? 0)
                    * 0.90m);

            return Math.Min(computerCost, 200000);
        }
        private decimal CalculatePCAEligibleEMI(DTOOnlineApplication model, DateTime retirmentDate)
        {
            int residualMonths = CalculateResidualMonth(retirmentDate);

            residualMonths -= 6;

            return Math.Max(0, Math.Min(48, residualMonths));
        }
        private decimal CalculatePCARepayingCapacity(DTOOnlineApplication model)
        {
            decimal credit =
                model.CommonData.TotalCredit ?? 0;

            decimal debit =
                model.CommonData.TotalDeductions ?? 0;

            return Math.Max(
                0,
                (credit * 0.75m) - debit);
        }
        private decimal CalculatePCADisbursement(DTOOnlineApplication model)
        {
            return Math.Round(
                (model.PCAApplication.PCA_Amount_Applied_For_Loan ?? 0)
                * 0.99m,
                0);
        }
        private decimal CalculatePCAEMI(DTOOnlineApplication model)
        {
            decimal principal =
                model.PCAApplication.PCA_Amount_Applied_For_Loan ?? 0;

            int months =
                Convert.ToInt32(
                    model.PCAApplication.PCA_EMI_Applied ?? 0);

            decimal monthlyRate =
                (8.50m / 12m) / 100m;

            if (principal <= 0 || months <= 0)
            {
                return 0;
            }

            double p = (double)principal;
            double r = (double)monthlyRate;
            double n = months;

            double emi =
                (p * r * Math.Pow(1 + r, n))
                / (Math.Pow(1 + r, n) - 1);

            return Math.Round((decimal)emi, 0);
        }


        // Retirement Validation Methods
        private int CalculateResidualMonth(DateTime retirementDate)
        {
            var today = DateTime.Today;
            if (retirementDate < today)
            {
                return 0;
            }
            int months = ((retirementDate.Year - today.Year) * 12) + retirementDate.Month - today.Month;
            if (retirementDate.Day < today.Day)
                months--;
            return Math.Max(0, months);
        }
        public decimal CalculateResidualService(DateTime retirementDate)
        {
            var today = DateTime.Today;

            if (retirementDate <= today)
                return 0;

            int years = retirementDate.Year - today.Year;

            if (retirementDate.Date < today.AddYears(years))
                years--;

            return years;
        }
        public DateTime CalculateRetirementDate(int userTypeId, int rankId, string prefix, int regtId, DateTime dob, DateTime doc, DateTime? promotionDate, bool extensionOfService, int retirementAge)
        {
            DateTime retirementDate;

            // Promotion Date Logic
            if (promotionDate.HasValue &&
                (rankId == 1 || rankId == 31))
            {
                retirementDate = promotionDate.Value.AddYears(4);
            }
            else
            {
                switch (userTypeId)
                {
                    case 1:
                        retirementDate = dob.AddYears(retirementAge);
                        break;

                    case 2:
                        retirementDate = doc.AddYears(10);
                        break;

                    case 3:
                    case 4:

                        if (rankId == 31 ||
                            rankId == 32 ||
                            rankId == 33)
                        {
                            retirementDate = dob.AddYears(retirementAge);
                        }
                        else
                        {
                            retirementDate = doc.AddYears(retirementAge);
                        }

                        break;

                    default:
                        throw new Exception("Invalid User Type");
                }
            }

            // Extension of Service Logic
            if ((prefix == "13" || prefix == "14")
                && extensionOfService)
            {
                retirementDate = retirementDate.AddYears(2);
            }

            return retirementDate;
        }
        public decimal CalculateTotalService(DateTime doc)
        {
            var today = DateTime.Today;

            var years = today.Year - doc.Year;

            if (doc.Date > today.AddYears(-years))
                years--;

            return years;
        }
        private async Task<DTORetirementInforesponse?> GetRetirementInfo(int rankId, int prefix, int regtId)
        {
            var prefixRankRetirementMap = new Dictionary<(int prefix, int rank), int>
            {
                { (11, 21), 57 },
                { (11, 22), 57 },
                { (11, 23), 57 },
                { (11, 24), 57 },
                { (11, 29), 57 },
                { (11, 26), 59 },
                { (11, 27), 60 },
                { (11, 28), 61 },

                { (3, 21), 57 },
                { (3, 22), 57 },
                { (3, 23), 57 },
                { (3, 24), 57 },
                { (3, 29), 57 },
                { (3, 26), 58 },
                { (3, 27), 59 },
                { (3, 28), 60 },
            };

            if (prefixRankRetirementMap.TryGetValue((prefix, rankId),
                out int retirementAge))
            {
                return new DTORetirementInforesponse
                {
                    RetirementAge = retirementAge,
                    UserTypeId = 1
                };
            }

            var userType = await _IMasterOnlyTable.GetUserType(prefix);
            var retAge = await _IMasterOnlyTable.GetRetirementAge(rankId, regtId);

            return new DTORetirementInforesponse
            {
                RetirementAge = retAge.FirstOrDefault()?.RetirementAge ?? 0,
                UserTypeId = userType.FirstOrDefault()?.UserType ?? 0
            };
        }
        Task<DTORetirementInforesponse?> IModelValidationService.GetRetirementInfo(int rankId, int prefix, int regtId)
        {
            return GetRetirementInfo(rankId, prefix, regtId);
        }
        public async Task ValidateClaimRetirementDetails(DTOClaimApplication model, ModelStateDictionary modelState)
        {
            var common = model.ClaimCommonData;

            if (common == null)
                return;

            var retirementInfo =
                await GetRetirementInfo(
                    common.DdlRank,
                    common.ArmyPrefix,
                    common.RegtCorps);

            if (retirementInfo == null)
            {
                modelState.AddModelError(
                    "",
                    "Unable to calculate retirement details.");

                return;
            }

            var exactRetirementDate =
                CalculateRetirementDate(
                    retirementInfo.UserTypeId,
                    common.DdlRank,
                    common.ArmyPrefix.ToString(),
                    common.RegtCorps,
                    common.DateOfBirth!.Value,
                    common.DateOfCommission!.Value,
                    common.DateOfPromotion,
                    common.ExtnOfService == "Yes",
                    retirementInfo.RetirementAge);

            var normalRetirementDate = new DateTime(exactRetirementDate.Year,exactRetirementDate.Month,DateTime.DaysInMonth(exactRetirementDate.Year,exactRetirementDate.Month));

            DateTime retirementDateToValidate;
            if (common.PrematureRetirement == true)
            {
                if (!common.DateOfRetirement.HasValue)
                {
                    modelState.AddModelError(
                        "ClaimCommonData.DateOfRetirement",
                        "Retirement date is required.");

                    return;
                }

                retirementDateToValidate =
                    common.DateOfRetirement.Value;

                if (retirementDateToValidate >= normalRetirementDate)
                {
                    modelState.AddModelError(
                        "ClaimCommonData.DateOfRetirement",
                        "Premature retirement date must be before normal retirement date.");
                }
            }
            else
            {
                retirementDateToValidate =
                    normalRetirementDate;

                if (common.DateOfRetirement == null ||
                    common.DateOfRetirement.Value.Date !=
                    normalRetirementDate.Date)
                {
                    modelState.AddModelError(
                        "ClaimCommonData.DateOfRetirement",
                        "Retirement date validation failed.");
                }
            }
            var totalService =
    CalculateTotalService(
        common.DateOfCommission.Value);

            if ((common.TotalService ?? 0)
                != (int)totalService)
            {
                modelState.AddModelError(
                    "ClaimCommonData.TotalService",
                    "Total service validation failed.");
            }
            var residualService =
    CalculateResidualService(
        retirementDateToValidate);

            if ((common.ResidualService ?? 0)
                != (int)residualService)
            {
                modelState.AddModelError(
                    "ClaimCommonData.ResidualService",
                    "Residual service validation failed.");
            }
            if (model.PropertyRenovation != null &&
    residualService > 1)
            {
                modelState.AddModelError(
                    "ClaimCommonData.ResidualService",
                    "Residual Service cannot exceed 2 years for Repair & Renovation.");
            }
        }
        public async Task ValidateLoanRetirementDetails(DTOOnlineApplication model, ModelStateDictionary modelState)
        {
            var common = model.CommonData;

            if (common == null)
                return;

            var retirementInfo =
                await GetRetirementInfo(
                    common.DdlRank,
                    common.ArmyPrefix,
                    common.RegtCorps);

            if (retirementInfo == null)
            {
                modelState.AddModelError("", "Unable to calculate retirement details.");
                return;
            }

            var calculatedRetirementDate =
                CalculateRetirementDate(
                    retirementInfo.UserTypeId,
                    common.DdlRank,
                    common.ArmyPrefix.ToString(),
                    common.RegtCorps,
                    common.DateOfBirth!.Value,
                    common.DateOfCommission!.Value,
                    common.DateOfPromotion,
                    common.ExtnOfService == "Yes",
                    retirementInfo.RetirementAge);

            // Retirement Date
            if (common.DateOfRetirement == null ||
                common.DateOfRetirement.Value.Date != calculatedRetirementDate.Date)
            {
                modelState.AddModelError(
                    "CommonData.DateOfRetirement",
                    "Retirement date validation failed.");
            }

            // Total Service
            var calculatedTotalService =
                CalculateTotalService(
                    common.DateOfCommission.Value);

            if (common.TotalService != calculatedTotalService)
            {
                modelState.AddModelError(
                    "CommonData.TotalService",
                    "Total service validation failed.");
            }

            // Residual Service
            var calculatedResidualService =
                CalculateResidualService(
                    calculatedRetirementDate);

            if (common.ResidualService != calculatedResidualService)
            {
                modelState.AddModelError(
                    "CommonData.ResidualService",
                    "Residual service validation failed.");
            }

            // Business Rule
            if (calculatedResidualService < 2)
            {
                modelState.AddModelError(
                    "CommonData.ResidualService",
                    "Residual service must be at least 2 years.");
            }
        }
    }
}
