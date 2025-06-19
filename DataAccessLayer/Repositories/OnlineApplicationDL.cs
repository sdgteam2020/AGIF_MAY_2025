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
using static System.Net.Mime.MediaTypeNames;
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

        public async Task<CommonDataonlineResponse> GetApplicationDetailsByArmyNo(string armyNumber, string Prefix, string Suffix, int appType)
        {
            var existingUser = await (from app in _context.trnApplications
                                      join doc in _context.trnDocumentUpload on app.ApplicationId equals doc.ApplicationId into docGroup
                                      from doc in docGroup.DefaultIfEmpty()
                                      where app.ApplicationType == appType && (app.ArmyPrefix.ToString() + app.Number + app.Suffix) == (armyNumber + Prefix + Suffix)
                                      select new CommonDataonlineResponse
                                      {
                                          ApplicationId = app.ApplicationId
                                      }).FirstOrDefaultAsync();


            return existingUser;
        }

        public async Task<bool> DeleteExistingLoan(string armyNumber, string Prefix, string Suffix, int appType)
        {
            var existingUser = await GetApplicationDetailsByArmyNo(armyNumber, Prefix, Suffix, appType);

            if (existingUser != null)
            {
                var carLoan = await _context.trnCar
                    .FirstOrDefaultAsync(c => c.ApplicationId == existingUser.ApplicationId);

                var hbaLoan = await _context.trnHBA
                    .FirstOrDefaultAsync(h => h.ApplicationId == existingUser.ApplicationId);

                var pcaLoan = await _context.trnPCA
                    .FirstOrDefaultAsync(p => p.ApplicationId == existingUser.ApplicationId);

                var documents = await _context.trnDocumentUpload
                    .Where(d => d.ApplicationId == existingUser.ApplicationId)
                    .ToListAsync();

                if (carLoan != null)
                    _context.trnCar.Remove(carLoan);

                if (hbaLoan != null)
                    _context.trnHBA.Remove(hbaLoan);

                if (pcaLoan != null)
                    _context.trnPCA.Remove(pcaLoan);

                if (documents.Any())
                    _context.trnDocumentUpload.RemoveRange(documents);

                var applicationEntity = await _context.trnApplications
                    .FirstOrDefaultAsync(a => a.ApplicationId == existingUser.ApplicationId);

                if (applicationEntity != null)
                    _context.trnApplications.Remove(applicationEntity);

                await _context.SaveChangesAsync();
                return true;

            }

            return false;
        }


        public Task<DTOCommonOnlineApplicationResponse> GetApplicationDetails(int applicationId)
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
                          join applicationType in _context.MApplicationTypes on common.ApplicationType equals applicationType.ApplicationTypeId 
                          where common.ApplicationId == applicationId
                          select new CommonDataonlineResponse
                          {
                              ParentUnit = parentUnit != null ? parentUnit.UnitName : string.Empty,
                              PresentUnit = presentUnit != null ? presentUnit.UnitName : string.Empty,
                              ApplicationId = common.ApplicationId,
                              ApplicationType = common.ApplicationType,
                              ApplicationTypeName= applicationType.ApplicationTypeName,
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
            string formtype = string.Empty;
            if (result != null)
            {
                //if(result.ApplicationType==1)
                if (result.ApplicationType == 1)
                {
                    formtype= "HBA";
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

                else if (result.ApplicationType == 2)
                {
                    formtype = "CA";
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

                else if (result.ApplicationType == 3)
                {
                    formtype = "PCA";
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

                var DocumentModel = _context.trnDocumentUpload.FirstOrDefault(x => x.ApplicationId == applicationId);

                if (DocumentModel != null)
                {
                    string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TempUploads", $"{formtype}_{result.Number}_{applicationId}");
                    List<DTODocumentFileView> lstdoc = new List<DTODocumentFileView>();
                    DTODocumentFileView dTODocumentFileView = new DTODocumentFileView();
                    if (DocumentModel.IsCancelledCheque)
                    {
                        dTODocumentFileView.FileName = DocumentModel.CancelledCheque;
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }
                    if (DocumentModel.IsSeviceExtnPdf)
                    {
                        dTODocumentFileView.FileName = DocumentModel.SeviceExtnPdf;
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }
                    if (DocumentModel.IsPaySlipPdf)
                    {
                        dTODocumentFileView.FileName = DocumentModel.PaySlipPdf;
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }
                    if (DocumentModel.IsQuotationPdf)
                    {
                        dTODocumentFileView.FileName = DocumentModel.QuotationPdf;
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }
                    if (DocumentModel.IsDrivingLicensePdf)
                    {
                        dTODocumentFileView.FileName = DocumentModel.DrivingLicensePdf;
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }

                    data.Documents = lstdoc; // Assign the list of documents to the response object
                    // Get all files in the directory

                }




            }

            return Task.FromResult(data);
        }

        public async Task<bool> UpdateApplicationStatus(int applicationId, int status)
        {
            var application = await _context.trnApplications.Where(i=>i.ApplicationId==applicationId).SingleOrDefaultAsync();
            if (application == null)
            {
                return false; // Just exit the method if not found
            }

            application.StatusCode = status;
            _context.trnApplications.Update(application);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateMergePdfStatus(int applicationId, bool status)
        {
            var application = await _context.trnApplications.Where(i => i.ApplicationId == applicationId).SingleOrDefaultAsync();
            if (application == null)
            {
                return false; // Just exit the method if not found
            }

            application.IsMergePdf = status;
            _context.trnApplications.Update(application);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CheckForCoRegister(string ArmyNo)
        {
            // Step 1: Get UserProfile by ArmyNo
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(u => u.ArmyNo == ArmyNo);

            if (userProfile == null)
                return false;

            // Step 2: Get UserMapping by ProfileId
            var userMapping = await _context.trnUserMappings
                .FirstOrDefaultAsync(m => m.ProfileId == userProfile.ProfileId);

            // Step 3: Return true if mapping exists, else false
            return userMapping != null;
        }

        public async Task<bool> CheckIsUnitRegister(string ArmyNo, int UnitId)
        {
            var userProfile = await _context.UserProfiles
               .FirstOrDefaultAsync(u => u.ArmyNo == ArmyNo);

            if (userProfile == null)
                return false;

            // Step 2: Get UserMapping by ProfileId
            var userMapping = await _context.trnUserMappings
                .FirstOrDefaultAsync(m => m.ProfileId == userProfile.ProfileId);

            // Step 3: Return true if mapping exists, else false
            if (userMapping.UnitId == UnitId)
                return true;
            else
                return false;

        }

        public async Task<bool> CheckIsCoRegister(int UnitId)
        {
            var user = await _context.trnUserMappings.Where(i => i.UnitId == UnitId && i.IsActive == true).FirstOrDefaultAsync();
            return user != null;
        }

        public async Task<bool> AddFwdCO(TrnFwdCO trnFwdCO)
        {
            await _context.TrnFwdCO.AddAsync(trnFwdCO);
            await SaveAsync();
            return false;
        }
    }
}
