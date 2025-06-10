using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace DataAccessLayer.Repositories
{
    public class OnlineApplicationDL : GenericRepositoryDL<CommonDataModel>, IOnlineApplication
    {
        protected new readonly ApplicationDbContext _context;

        public OnlineApplicationDL(ApplicationDbContext context) : base(context)
        {
            _context = context;
           
        }

        public Task<bool> IsUser(string AadharNo)
        {
           _context.OnlineApplications.ToList();
            var user = _context.OnlineApplications.FirstOrDefault(x => x.AadharNo == AadharNo);
            if (user != null)
            {
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }
        public Task<DateTime> GetRetirementDate(int rankId, int Prefix, DateTime dateTime)
        {
            var userType = _context.MArmyPrefixes.FirstOrDefault(x => x.Id == Prefix);
            var retAge = _context.MRanks.FirstOrDefault(x => x.RankId == rankId);
            var retirementAge = retAge.RetirementAge;
            var userTypeId = userType.UserType;

            var ret = (from prefix in _context.MArmyPrefixes
                       join rank in _context.MRanks on rankId equals Prefix
                       where prefix.Id == Prefix
                       select new
                       {
                           userType = prefix.UserType,
                           retAge = rank.RetirementAge

                       });


            if (retirementAge > 0 && userTypeId != 0)
            {
                DateTime retirementDate = DateTime.Now.AddYears((int)retirementAge);
                return Task.FromResult(retirementDate);
            }
            else
            {
                return Task.FromResult(DateTime.MinValue);
            }
        }

        public Task<CommonOnlineApplicationResponse?> GetApplicationDetails(int applicationId,string formtype)
        {
            int Applicationtype = 0;

            

            var commonDataModel = _context.Applications.FirstOrDefault(x => x.ApplicationId == applicationId);
            if (commonDataModel == null)
            {
                return Task.FromResult<CommonOnlineApplicationResponse?>(null);
            }
            else
            {
               
                var result = (from common in _context.Applications
                              join prefix in _context.MArmyPrefixes on common.ArmyPrefix equals prefix.Id into prefixGroup
                              from prefix in prefixGroup.DefaultIfEmpty()
                              join oldPrefix in _context.MArmyPrefixes on common.OldArmyPrefix equals oldPrefix.Id into oldPrefixGroup
                              from oldPrefix in oldPrefixGroup.DefaultIfEmpty()
                              join rank in _context.MRanks on common.DdlRank equals rank.RankId into rankGroup
                              from rank in rankGroup.DefaultIfEmpty()
                              join armyPostOffice in _context.MArmyPostOffices on common.ArmyPostOffice equals armyPostOffice.Id into armyPostOfficeGroup
                              from armyPostOffice in armyPostOfficeGroup.DefaultIfEmpty()
                              join regCorps in _context.MRegtCorps on common.RegtCorps equals regCorps.Id into regCorpsGroup
                              from regCorps in regCorpsGroup.DefaultIfEmpty()
                              join parentUnit in _context.MUnits on common.ParentUnit equals parentUnit.UnitId into parentUnitGroup
                              from parentUnit in parentUnitGroup.DefaultIfEmpty()
                              join presentUnit in _context.MUnits on common.PresentUnit equals presentUnit.UnitId into presentUnitGroup
                              from presentUnit in presentUnitGroup.DefaultIfEmpty()
                              select new
                              {
                                  PrefixName = prefix != null ? prefix.Prefix : string.Empty,
                                  OldArmyPrefix = oldPrefix != null ? oldPrefix.Prefix : string.Empty,
                                  Rank = rank != null ? rank.RankName : string.Empty,
                                  ArmyPostOffice = armyPostOffice != null ? armyPostOffice.ArmyPostOffice : string.Empty,
                                  RegCorps = regCorps != null ? regCorps.RegtName : string.Empty,
                                  ParentUnit = parentUnit != null ? parentUnit.UnitName : string.Empty,
                                  PresentUnit = presentUnit != null ? presentUnit.UnitName : string.Empty
                              }).FirstOrDefault();

                if (formtype == "HBA")
                    Applicationtype = 1;
                else if (formtype == "CA")
                    Applicationtype = 2;
                else if (formtype == "PCA")
                    Applicationtype = 3;

                var Hbamodel = _context.HBA.FirstOrDefault(x => x.ApplicationId == applicationId);
                var Carmodel= _context.Car.FirstOrDefault(x => x.ApplicationId == applicationId);
                var Pcamodel= _context.Car.FirstOrDefault(x => x.ApplicationId == applicationId);

                var Hba = _context.MLoanTypes
                    .Where(x => x.Id == Hbamodel.PropertyType)
                    .Select(x => x.LoanType)
                    .FirstOrDefault() ?? string.Empty;

                var Car = _context.MLoanTypes
                   .Where(x => x.Id == Hbamodel.PropertyType)
                   .Select(x => x.LoanType)
                   .FirstOrDefault() ?? string.Empty;



                var response = new CommonOnlineApplicationResponse
                {
                    OnlineApplicationResponse = new OnlineApplicationResponse
                    {
                        ApplicationId = commonDataModel.ApplicationId,
                        ArmyPrefix = commonDataModel.ArmyPrefix,
                        Number = $"{result?.PrefixName}{commonDataModel.Number ?? string.Empty}{commonDataModel.Suffix ?? string.Empty}".Trim(),
                        Suffix = commonDataModel.Suffix ?? string.Empty,
                        OldArmyPrefix = commonDataModel.OldArmyPrefix,
                        OldNumber = $"{result?.OldArmyPrefix}{commonDataModel.OldNumber ?? string.Empty}{commonDataModel.OldSuffix ?? string.Empty}".Trim(),
                        OldSuffix = commonDataModel.OldSuffix ?? string.Empty,
                        DdlRank = result?.Rank ?? string.Empty,
                        ApplicantName = commonDataModel.ApplicantName ?? string.Empty,
                        DateOfBirth = commonDataModel.DateOfBirth,
                        NextFmnHQ = commonDataModel.NextFmnHQ ?? string.Empty,
                        ArmyPostOffice = result?.ArmyPostOffice ?? string.Empty,
                        RegtCorps = result?.RegCorps ?? string.Empty,
                        ParentUnit = result?.ParentUnit ?? string.Empty,
                        PresentUnit = result?.PresentUnit ?? string.Empty,
                        PresentUnitPin = commonDataModel.PresentUnitPin ?? string.Empty,
                        Vill_Town = commonDataModel.Vill_Town ?? string.Empty,
                        PostOffice = commonDataModel.PostOffice ?? string.Empty,
                        Distt = commonDataModel.Distt ?? string.Empty,
                        State = commonDataModel.State ?? string.Empty,
                        DateOfCommission = commonDataModel.DateOfCommission,
                        DateOfPromotion = commonDataModel.DateOfPromotion,
                        DateOfRetirement = commonDataModel.DateOfRetirement,
                        AadharCardNo = commonDataModel.AadharCardNo ?? string.Empty,
                        PanCardNo = commonDataModel.PanCardNo ?? string.Empty,
                        MobileNo = commonDataModel.MobileNo ?? string.Empty,
                        Email = commonDataModel.Email ?? string.Empty,
                        Code = commonDataModel.Code ?? string.Empty,
                        SalaryAcctNo = commonDataModel.SalaryAcctNo ?? string.Empty,
                        IfsCode = commonDataModel.IfsCode ?? string.Empty,
                        NameOfBank = commonDataModel.NameOfBank ?? string.Empty,
                        NameOfBankBranch = commonDataModel.NameOfBankBranch ?? string.Empty,
                        pcda_pao = commonDataModel.pcda_pao ?? string.Empty,
                    },
                    CarApplicationResponse = new DTOCarApplicationresponse
                    {
                        // Fill in car application data from the `Carmodel` 
                        // Example:
                        DealerName = Carmodel?.DealerName ?? string.Empty,
                        Veh_Loan_Type = Car ?? string.Empty,
                        CompanyName = Carmodel?.CompanyName ?? string.Empty,
                        ModelName = Carmodel?.ModelName ?? string.Empty,
                        VehicleCost = Carmodel?.VehicleCost ?? 0,
                        CA_LoanFreq = Carmodel?.CA_LoanFreq ?? 0,
                        DrivingLicenseNo = Carmodel?.DrivingLicenseNo ?? string.Empty,
                        
                        // Add other fields as needed
                    },
                    HbaApplicationResponse = new DTOHbaApplicationresponse
                    {
                        // Fill in HBA application data from the `Hbamodel` 
                        // Example:
                        PropertyType = Hba ?? string.Empty,
                        PropertyAddress = Hbamodel?.PropertyAddress ?? string.Empty,
                        PropertyCost = Hbamodel?.PropertyCost ?? 0,
                        HBA_LoanFreq = Hbamodel?.HBA_LoanFreq ?? string.Empty,
                        // Add other fields as needed
                    }
                };

                return Task.FromResult<CommonOnlineApplicationResponse?>(response);
            }
        }
    }

}
