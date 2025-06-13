using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer.Repositories
{
    public class OnlineApplicationDL : GenericRepositoryDL<CommonDataModel>, IOnlineApplication
    {
        protected new readonly ApplicationDbContext _context;

        public OnlineApplicationDL(ApplicationDbContext context) : base(context)
        {
            _context = context;
           
        }

        //public Task<bool> IsUser(string AadharNo)
        //{
        //   _context.OnlineApplications.ToList();
        //    var user = _context.OnlineApplications.FirstOrDefault(x => x.AadharNo == AadharNo);
        //    if (user != null)
        //    {
        //        return Task.FromResult(true);
        //    }
        //    else
        //    {
        //        return Task.FromResult(false);
        //    }
        //}
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

        public Task<DTOCommonOnlineApplicationResponse> GetApplicationDetails(int applicationId, string formtype)
        {
            DTOCommonOnlineApplicationResponse data = new DTOCommonOnlineApplicationResponse();

            var result = (from common in _context.trnApplications
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
                          where common.ApplicationId == applicationId
                          select new CommonDataonlineResponse
                          {
                              ParentUnit = parentUnit != null ? parentUnit.UnitName : string.Empty,
                              PresentUnit = presentUnit != null ? presentUnit.UnitName : string.Empty,
                              ApplicationId = common.ApplicationId,
                              ApplicationType = common.ApplicationType,
                              ArmyPrefix = common.ArmyPrefix,
                              Number = $"{(prefix != null ? prefix.Prefix : string.Empty)}{common.Number ?? string.Empty}{common.Suffix ?? string.Empty}".Trim(),
                              AadharCardNo = common.AadharCardNo ?? string.Empty,
                              Suffix = common.Suffix ?? string.Empty,
                              OldArmyPrefix = common.OldArmyPrefix,
                              OldNumber = $"{(oldPrefix != null ? oldPrefix.Prefix : string.Empty)}{common.OldNumber ?? string.Empty}{common.OldSuffix ?? string.Empty}".Trim(),
                              OldSuffix = common.OldSuffix ?? string.Empty,
                              DdlRank = rank != null ? rank.RankName : string.Empty,
                              ApplicantName = common.ApplicantName ?? string.Empty,
                              DateOfBirth = common.DateOfBirth,
                              DateOfCommission = common.DateOfCommission,
                              NextFmnHQ = common.NextFmnHQ ?? string.Empty,
                              ArmyPostOffice = armyPostOffice != null ? armyPostOffice.ArmyPostOffice : string.Empty,
                              RegtCorps = regCorps != null && regCorps.RegtName != null ? regCorps.RegtName : string.Empty,
                              PresentUnitPin = common.PresentUnitPin ?? string.Empty,
                              Vill_Town = common.Vill_Town ?? string.Empty,
                              PostOffice = common.PostOffice ?? string.Empty,
                              Distt = common.Distt ?? string.Empty,
                              State = common.State ?? string.Empty,
                              DateOfPromotion = common.DateOfPromotion,
                              DateOfRetirement = common.DateOfRetirement,
                              PanCardNo = common.PanCardNo ?? string.Empty,
                              MobileNo = common.MobileNo ?? string.Empty,
                              Email = common.Email ?? string.Empty,
                              Code = common.Code ?? string.Empty,
                              SalaryAcctNo = common.SalaryAcctNo ?? string.Empty,
                              IfsCode = common.IfsCode ?? string.Empty,
                              NameOfBank = common.NameOfBank ?? string.Empty,
                              NameOfBankBranch = common.NameOfBankBranch ?? string.Empty,
                              pcda_pao = common.pcda_pao ?? string.Empty,
                          }).FirstOrDefault();

            if (result != null)
            {
                //if(result.ApplicationType==1)
                if(result.ApplicationType == 1)
                {
                   
                    var Hbamodel = (from hba in _context.trnHBA
                                     join loanType in _context.MLoanTypes on hba.PropertyType equals loanType.Id into loanTypeGroup
                                     from loanType in loanTypeGroup.DefaultIfEmpty()
                                     where hba.ApplicationId == applicationId
                                     select new DTOHbaApplicationresponse
                                     {
                                         PropertyType = loanType != null ? loanType.LoanType : string.Empty, // Getting LoanType from MLoanTypes
                                         PropertySeller = hba.PropertySeller.ToString(),
                                         PropertyAddress = hba.PropertyAddress,
                                         PropertyCost = hba.PropertyCost,
                                         HBA_LoanFreq = hba.HBA_LoanFreq
                                     }).FirstOrDefault();

                    data.OnlineApplicationResponse = result; // Assuming result is already defined

                    // Directly assign the DTO
                    data.HbaApplicationResponse = Hbamodel;
                }

                else if(result.ApplicationType == 2)
                 {
                    var Carmodel = (from car in _context.trnCar
                                    join loanType in _context.MLoanTypes on car.Veh_Loan_Type equals loanType.Id into loanTypeGroup
                                    from loanType in loanTypeGroup.DefaultIfEmpty()
                                    where car.ApplicationId == applicationId
                                    select new DTOCarApplicationresponse
                                    {
                                        DealerName = car.DealerName,
                                        Veh_Loan_Type = loanType != null ? loanType.LoanType : string.Empty, // Get LoanType from MLoanTypes
                                        CompanyName = car.CompanyName,
                                        ModelName = car.ModelName,
                                        CA_LoanFreq = car.CA_LoanFreq,
                                        CA_Amount_Applied_For_Loan = car.CA_Amount_Applied_For_Loan,
                                    }).FirstOrDefault();

                    data.OnlineApplicationResponse = result;
                    data.CarApplicationResponse = Carmodel;
                }

                else if(result.ApplicationType == 3)
                {
                    var PcaModal = (from pca in _context.trnPCA
                                    join loanType in _context.MLoanTypes on pca.computer_Loan_Type equals loanType.Id into loanTypeGroup
                                    from loanType in loanTypeGroup.DefaultIfEmpty()
                                    where pca.ApplicationId == applicationId
                                    select new DTOPCAApplicationresponse
                                    {
                                        computer_Loan_Type = loanType != null ? loanType.LoanType : string.Empty, // Getting LoanType from MLoanTypes
                                        PCA_dealerName = pca.PCA_dealerName,
                                        PCA_companyName = pca.PCA_companyName,
                                        computerCost = pca.computerCost,
                                        PCA_LoanFreq = pca.PCA_LoanFreq
                                    }).FirstOrDefault();

                    data.OnlineApplicationResponse = result; // Assuming result is already defined

                    // Directly assign the DTO
                    data.PcaApplicationResponse = PcaModal;
                }               

                var DocumentModel= _context.trnDocumentUpload.FirstOrDefault(x => x.ApplicationId == applicationId);

                if(DocumentModel!=null)
                {
                    string ArmyNo = $"{result.Number}";
                    string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TempUploads", $"{formtype}_{ArmyNo}_{applicationId}");

                    // Get all files in the directory
                    if (Directory.Exists(directoryPath))
                    {
                        // Get all files in the directory
                        var fileNames = Directory.GetFiles(directoryPath);

                        // Prepare documents for the view
                        data.Documents = fileNames.Select(filePath => new DTODocumentFileView
                        {
                            FileName = Path.GetFileName(filePath),
                            FilePath = Path.Combine("/TempUploads", formtype + "_" + ArmyNo + "_" + applicationId, Path.GetFileName(filePath))
                        }).ToList();
                    }
                }
              



            }

            return Task.FromResult(data);
        }
    }

}
