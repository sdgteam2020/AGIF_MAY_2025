using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer.Repositories
{
    public static class ListExtensions
    {
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row); // ✅ Add the fully populated DataRow
            }

            return table;
        }

    }
    public class OnlineApplicationDL : GenericRepositoryDL<CommonDataModel>, IOnlineApplication
    {
        protected new readonly ApplicationDbContext _context;

        public OnlineApplicationDL(ApplicationDbContext context) : base(context)
        {
            _context = context;

        }

        public async Task<string> GetFormType(int ApplicationID)
        {
            var application = await _context.trnApplications
                .Where(a => a.ApplicationId == ApplicationID)
                .Select(a => a.ApplicationType)
                .FirstOrDefaultAsync();
            if(application!=null && application!=0)
            {
                return application switch
                {
                    1 => "HBA",
                    2 => "CA",
                    3 => "PCA",
                    _ => ""
                };
            }
            return string.Empty;
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

                var AddressDetails = await _context.trnAddressDetails
                    .FirstOrDefaultAsync(a => a.ApplicationId == existingUser.ApplicationId);

                var AccountDetails = await _context.trnAccountDetails
                    .FirstOrDefaultAsync(a => a.ApplicationId == existingUser.ApplicationId);

                if (carLoan != null)
                    _context.trnCar.Remove(carLoan);

                if (hbaLoan != null)
                    _context.trnHBA.Remove(hbaLoan);

                if (pcaLoan != null)
                    _context.trnPCA.Remove(pcaLoan);

                if (documents.Any())
                    _context.trnDocumentUpload.RemoveRange(documents);

                if(AddressDetails != null)
                    _context.trnAddressDetails.Remove(AddressDetails);

                if (AccountDetails != null)
                    _context.trnAccountDetails.Remove(AccountDetails);

                var applicationEntity = await _context.trnApplications
                    .FirstOrDefaultAsync(a => a.ApplicationId == existingUser.ApplicationId);

                if (applicationEntity != null)
                    _context.trnApplications.Remove(applicationEntity);

                await _context.SaveChangesAsync();
                return true;

            }

            return false;
        }

        public Task<DTOCommonOnlineApplicationResponse> GetUnitByApplicationId(int applicationId)
        {
            DTOCommonOnlineApplicationResponse data = new DTOCommonOnlineApplicationResponse();

            var application = _context.trnApplications
                .Where(x=>x.ApplicationId == applicationId)
                .Select(x=>new { x.ApplicationId, x.IOArmyNo })
                .FirstOrDefault();

            if(application == null)
            {
                return Task.FromResult(data); // return empty if not found
            }

            if(string.IsNullOrEmpty(application.IOArmyNo))
            {
                var result = (from appl in _context.trnApplications
                              where appl.ApplicationId == applicationId
                              join unit in _context.MUnits on appl.PresentUnit equals unit.UnitId
                              join mapping in _context.trnUserMappings on unit.UnitId equals mapping.UnitId
                              join profile in _context.UserProfiles on mapping.ProfileId equals profile.ProfileId
                              where mapping.IsActive == true && mapping.IsPrimary == true
                              join Rank in _context.MRanks on profile.rank equals Rank.RankId
                              select new CommonDataonlineResponse
                              {
                                  PresentUnit = unit.UnitName,
                                  ApplicationId = appl.ApplicationId,
                                  Number = profile.ArmyNo,
                                  CoName = profile.Name,
                                  DdlRank = Rank.RankName,
                              }).FirstOrDefault();
                data.OnlineApplicationResponse = result;
            }
            else
            {
                var ioResult = (from appl in _context.trnApplications
                                where appl.ApplicationId == applicationId
                                join profile in _context.UserProfiles on appl.IOArmyNo equals profile.ArmyNo
                                join mapping in _context.trnUserMappings on profile.ProfileId equals mapping.ProfileId
                                join unit in _context.MUnits on mapping.UnitId equals unit.UnitId
                                where mapping.IsActive == true
                                join rank in _context.MRanks on profile.rank equals rank.RankId
                                select new CommonDataonlineResponse
                                {
                                    PresentUnit = unit.UnitName,
                                    ApplicationId = appl.ApplicationId,
                                    Number = profile.ArmyNo,
                                    CoName = profile.Name,
                                    DdlRank = rank.RankName,
                                }).FirstOrDefault();
                data.OnlineApplicationResponse = ioResult;
            }
            return Task.FromResult(data);
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

                          join AddressDetails in _context.trnAddressDetails on common.ApplicationId equals AddressDetails.ApplicationId into AddressDetailsModelGroup
                          from AddressDetails in AddressDetailsModelGroup.DefaultIfEmpty()
                          join AccountDetails in _context.trnAccountDetails on common.ApplicationId equals AccountDetails.ApplicationId into AccountDetailsModelGroup
                          from AccountDetails in AccountDetailsModelGroup.DefaultIfEmpty()

                          where common.ApplicationId == applicationId
                          select new CommonDataonlineResponse
                          {
                              ParentUnit = parentUnit != null ? parentUnit.UnitName : string.Empty,
                              PresentUnit = presentUnit != null ? presentUnit.UnitName : string.Empty,
                              ApplicationId = common.ApplicationId,
                              ApplicationType = common.ApplicationType,
                              ApplicationTypeName = applicationType.ApplicationTypeName,
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
                              Vill_Town = AddressDetails.Vill_Town ?? string.Empty,
                              PostOffice = AddressDetails.PostOffice ?? string.Empty,
                              Distt = AddressDetails.Distt ?? string.Empty,
                              State = AddressDetails.State ?? string.Empty,
                              DateOfPromotion = common.DateOfPromotion,
                              DateOfRetirement = common.DateOfRetirement,
                              PanCardNo = common.PanCardNo ?? string.Empty,
                              MobileNo = common.MobileNo ?? string.Empty,
                              Email = common.Email ?? string.Empty,
                              Code = AddressDetails.Code ?? string.Empty,
                              SalaryAcctNo = AccountDetails.SalaryAcctNo ?? string.Empty,
                              IfsCode = AccountDetails.IfsCode ?? string.Empty,
                              NameOfBank = AccountDetails.NameOfBank ?? string.Empty,
                              NameOfBankBranch = AccountDetails.NameOfBankBranch ?? string.Empty,
                              pcda_pao = common.pcda_pao ?? string.Empty,
                              pcda_AcctNo = common.pcda_AcctNo ?? string.Empty,
                              CivilPostalAddress = common.CivilPostalAddress ?? string.Empty,
                              ConfirmSalaryAcctNo= AccountDetails.ConfirmSalaryAcctNo,
                              UpdatedOn = common.UpdatedOn.ToString(),
                              EmailDomain = common.EmailDomain ?? string.Empty,
                              
                              
                          }).FirstOrDefault();
            string formtype = string.Empty;
            if (result != null)
            {
                if (result.ApplicationType == 1)
                {
                    formtype = "HBA";
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
                                        HBA_LoanFreq = hba.HBA_LoanFreq,
                                        HBA_Amount_Applied_For_Loan = hba.HBA_Amount_Applied_For_Loan,
                                        HBA_EMI_Applied = hba.HBA_EMI_Applied,
                                        HBA_approxEMIAmount = hba.HBA_approxEMIAmount,
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
                                        VehicleCost=car.VehicleCost,
                                        CA_EMI_Applied = car.CA_EMI_Applied,
                                        CA_approxEMIAmount = car.CA_approxEMIAmount,
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
                                        PCA_LoanFreq = pca.PCA_LoanFreq,
                                        PCA_modelName=pca.PCA_modelName,
                                        PCA_Amount_Applied_For_Loan=pca.PCA_Amount_Applied_For_Loan,
                                        PCA_EMI_Applied = pca.PCA_EMI_Applied,
                                        PCA_approxEMIAmount = pca.PCA_approxEMIAmount,
                                    }).FirstOrDefault();

                    data.OnlineApplicationResponse = result; // Assuming result is already defined

                    // Directly assign the DTO
                    data.PcaApplicationResponse = PcaModal;
                }

                var DocumentModel = _context.trnDocumentUpload.FirstOrDefault(x => x.ApplicationId == applicationId);

                if (DocumentModel != null)
                {
                    if (formtype == "CA")
                    {
                        if(data.CarApplicationResponse.Veh_Loan_Type == "Two Wheeler")
                        {
                            formtype = "TW";
                        }
                    }
                    
                    string directoryPath = Path.Combine("/TempUploads", $"{formtype}{result.Number}_{applicationId}");
                    List<DTODocumentFileView> lstdoc = new List<DTODocumentFileView>();

                    if (DocumentModel.IsCancelledCheque)
                    {
                        DTODocumentFileView dTODocumentFileView = new DTODocumentFileView();
                        dTODocumentFileView.FileName = DocumentModel.CancelledCheque + ".Pdf";
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }
                    if (DocumentModel.IsSeviceExtnPdf)
                    {
                        DTODocumentFileView dTODocumentFileView = new DTODocumentFileView();
                        dTODocumentFileView.FileName = DocumentModel.SeviceExtnPdf + ".Pdf";
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }
                    if (DocumentModel.IsPaySlipPdf)
                    {
                        DTODocumentFileView dTODocumentFileView = new DTODocumentFileView();
                        dTODocumentFileView.FileName = DocumentModel.PaySlipPdf + ".Pdf";
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }
                    if (DocumentModel.IsQuotationPdf)
                    {
                        DTODocumentFileView dTODocumentFileView = new DTODocumentFileView();
                        dTODocumentFileView.FileName = DocumentModel.QuotationPdf + ".Pdf";
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }
                    if (DocumentModel.IsDrivingLicensePdf)
                    {
                        DTODocumentFileView dTODocumentFileView = new DTODocumentFileView();
                        dTODocumentFileView.FileName = DocumentModel.DrivingLicensePdf + ".Pdf";
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
            var application = await _context.trnApplications.Where(i => i.ApplicationId == applicationId).SingleOrDefaultAsync();
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

        public async Task<bool> CheckIsUnitRegister(string ArmyNo)
        {
            var userProfile = await _context.UserProfiles
               .FirstOrDefaultAsync(u => u.ArmyNo == ArmyNo);

            if (userProfile == null)
                return false;
            else
                return true;
            
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

        public async Task<UserMapping?> GetIODetails(string ArmyNumber)
        {
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(u => u.ArmyNo == ArmyNumber);

            if (userProfile == null)
                return null;

            var userMapping = await _context.trnUserMappings
                .FirstOrDefaultAsync(m => m.ProfileId == userProfile.ProfileId);

            return userMapping;
        }

        public async Task<UserMapping?> GetUserDetails(string CoArmyNumber)
        {
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(u => u.ArmyNo == CoArmyNumber);

            if (userProfile == null)
                return null;

            var userMapping = await _context.trnUserMappings
                .FirstOrDefaultAsync(m => m.ProfileId == userProfile.ProfileId);

            return userMapping;
        }

        public async Task<UserMapping?> GetCoDetails(int applicationId)
        {
            // Step 1: Get the application by applicationId
            var application = await _context.trnApplications
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (application == null)
                return null;

            // Step 3: Get the UserMapping for the PresentUnit where IsPrimary and IsActive are true
            var userMapping = await _context.trnUserMappings
                .FirstOrDefaultAsync(m => m.UnitId == application.PresentUnit && m.IsPrimary);

            return userMapping;
        }

        //public async Task<string> GetCOName(int mappingId)
        //{
        //    // Get the UserMapping by MappingId
        //    var userMapping = await _context.trnUserMappings.FirstOrDefaultAsync(m => m.MappingId == mappingId);

            
        //    if (userMapping == null)
        //        return string.Empty;

        //    // Get the UserProfile by ProfileId from UserMapping
        //    var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.ProfileId == userMapping.MappingId);
        //    if (userProfile == null)
        //        return string.Empty;

        //    // Get the RankName from MRanks using rank id from UserProfile
        //    var rank = await _context.MRanks.FirstOrDefaultAsync(r => r.RankId == userProfile.rank);
        //    string rankName = rank != null ? rank.RankName : string.Empty;

        //    // Concatenate rankName and userName
        //    return $"{rankName} {userProfile.userName}".Trim();
        //}
        public async Task<(string Name, string Mobile,string Armyno)> GetCODetails(int mappingId)
        {
            // Get the UserMapping by MappingId
            var userMapping = await _context.trnUserMappings.FirstOrDefaultAsync(m => m.MappingId == mappingId);
            if (userMapping == null)
                return (string.Empty, string.Empty,string.Empty);

            // Get the UserProfile by ProfileId from UserMapping
            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.ProfileId == userMapping.MappingId);
            if (userProfile == null)
                return (string.Empty, string.Empty, string.Empty);

            // Get the RankName from MRanks using rank id from UserProfile
            var rank = await _context.MRanks.FirstOrDefaultAsync(r => r.RankId == userProfile.rank);
            string rankName = rank != null ? rank.RankName : string.Empty;

         

            // Concatenate rankName and userName
            string fullName = $"{rankName} {userProfile.Name}".Trim();
            string mobile = userProfile.MobileNo ?? string.Empty; // Assuming MobileNo is the field name

            var Armyno=userProfile.ArmyNo ?? string.Empty;

            return (fullName, mobile,Armyno);
        }

        public async Task<bool> CheckExtensionofservice(int applicationid)
        {
            // Fetch the application record by applicationid
            var application = await _context.trnApplications
                .Where(a => a.ApplicationId == applicationid).FirstOrDefaultAsync();

            if (application == null)
                return false;

            if(string.IsNullOrEmpty(application.ExtnOfService))
                return false;
            else if(application.ExtnOfService=="Yes")
                return true;
            else if (application.ExtnOfService == "No")
                return false;
            else
                return false;

        }


        public async Task<bool> CheckDocumentUploaded(int ApplicationID)
        {
            var document = await _context.trnDocumentUpload
                .FirstOrDefaultAsync(d => d.ApplicationId == ApplicationID);
            return document != null;
        }



        public Task<string?> GetIOArmyNoAsync(int applicationId)
        {
            var application = _context.trnApplications
                .FirstOrDefault(i => i.ApplicationId == applicationId);

            if (application == null || string.IsNullOrWhiteSpace(application.IOArmyNo))
            {
                return Task.FromResult<string?>(null);
            }

            return Task.FromResult(application.IOArmyNo);
        }
        public Task<DTOCommonOnlineApplicationResponseList> GetApplicationDetailsForExport(DTOExportRequest dTOExport)
        {
            DTOCommonOnlineApplicationResponseList data = new DTOCommonOnlineApplicationResponseList();

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

                          join AddressDetails in _context.trnAddressDetails on common.ApplicationId equals AddressDetails.ApplicationId into AddressDetailsModelGroup
                          from AddressDetails in AddressDetailsModelGroup.DefaultIfEmpty()
                          join AccountDetails in _context.trnAccountDetails on common.ApplicationId equals AccountDetails.ApplicationId into AccountDetailsModelGroup
                          from AccountDetails in AccountDetailsModelGroup.DefaultIfEmpty()


                          where dTOExport.Id.Contains(common.ApplicationId)
                          select new CommonDataonlineResponse
                          {
                              ParentUnit = parentUnit != null ? parentUnit.UnitName : string.Empty,
                              PresentUnit = presentUnit != null ? presentUnit.UnitName : string.Empty,
                              ApplicationId = common.ApplicationId,
                              ApplicationType = common.ApplicationType,
                              ApplicationTypeName = applicationType.ApplicationTypeName,
                              ApplicationTypeAbbr = applicationType.ApplicationTypeAbbr ?? string.Empty,
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
                              Vill_Town = AddressDetails.Vill_Town ?? string.Empty,
                              PostOffice = AddressDetails.PostOffice ?? string.Empty,
                              Distt = AddressDetails.Distt ?? string.Empty,
                              State = AddressDetails.State ?? string.Empty,
                              DateOfPromotion = common.DateOfPromotion,
                              DateOfRetirement = common.DateOfRetirement,
                              PanCardNo = common.PanCardNo ?? string.Empty,
                              MobileNo = common.MobileNo ?? string.Empty,
                              Email = common.Email ?? string.Empty,
                              Code = AddressDetails.Code ?? string.Empty,
                              SalaryAcctNo = AccountDetails.SalaryAcctNo ?? string.Empty,
                              IfsCode = AccountDetails.IfsCode ?? string.Empty,
                              NameOfBank = AccountDetails.NameOfBank ?? string.Empty,
                              NameOfBankBranch = AccountDetails.NameOfBankBranch ?? string.Empty,
                              pcda_pao = common.pcda_pao ?? string.Empty,
                              pcda_AcctNo = common.pcda_AcctNo ?? string.Empty,
                          }).ToListAsync();
            data.OnlineApplicationResponse = result.Result; // Assuming result is already defined

            return Task.FromResult(data);
        }


        public Task<DataTable> GetApplicationDetailsForExcel(DTOExportRequest dTOExport)
        {
           
            DataTable dataTable = new DataTable();
            var query = (from common in _context.trnApplications
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
                         join car in _context.trnCar on common.ApplicationId equals car.ApplicationId into carGroup
                         from car in carGroup.DefaultIfEmpty()
                         join pca in _context.trnPCA on common.ApplicationId equals pca.ApplicationId into pcaGroup
                         from pca in pcaGroup.DefaultIfEmpty()
                         join hba in _context.trnHBA on common.ApplicationId equals hba.ApplicationId into hbaGroup
                         from hba in hbaGroup.DefaultIfEmpty()
                         join AddressDetails in _context.trnAddressDetails on common.ApplicationId equals AddressDetails.ApplicationId into AddressDetailsModelGroup
                         from AddressDetails in AddressDetailsModelGroup.DefaultIfEmpty()
                         join AccountDetails in _context.trnAccountDetails on common.ApplicationId equals AccountDetails.ApplicationId into AccountDetailsModelGroup
                         from AccountDetails in AccountDetailsModelGroup.DefaultIfEmpty()
                         where dTOExport.Id.Contains(common.ApplicationId)

                                    select new DTOExcelResponse
                                    {
                                        Unit = presentUnit != null ? presentUnit.UnitName : string.Empty,
                                       
                                        apfx = prefix.Prefix,
                                        ano = $"{(prefix != null ? prefix.Prefix : string.Empty)}{common.Number ?? string.Empty}{common.Suffix ?? string.Empty}".Trim(),
                                        AadharNo = common.AadharCardNo ?? string.Empty,
                                        asfx = common.Suffix ?? string.Empty,
                                        opfx = prefix.Prefix,
                                        ono = $"{(oldPrefix != null ? oldPrefix.Prefix : string.Empty)}{common.OldNumber ?? string.Empty}{common.OldSuffix ?? string.Empty}".Trim(),
                                        osfx = common.OldSuffix ?? string.Empty,
                                        Rank = rank != null ? rank.RankName : string.Empty,
                                        Loanee_Name = common.ApplicantName ?? string.Empty,
                                        Date_Of_Birth = common.DateOfBirth,
                                        Enrollment_Date = common.DateOfCommission,
                                        Regt_Corps = regCorps != null && regCorps.RegtName != null ? regCorps.RegtName : string.Empty,
                                        Pers_Address_Line1 = AddressDetails.Vill_Town ?? string.Empty,
                                        Pers_Address_Line2 = AddressDetails.PostOffice ?? string.Empty,
                                        Pers_Address_Line3 = AddressDetails.Distt ?? string.Empty,
                                        Pers_Address_Line4 = AddressDetails.State ?? string.Empty,
                                        Promotion_Date = common.DateOfPromotion,
                                        Retirement_Date = common.DateOfRetirement,
                                        PANNo = common.PanCardNo ?? string.Empty,
                                        Mobile_No = common.MobileNo ?? string.Empty,
                                        Payee_Account_No = AccountDetails.SalaryAcctNo ?? string.Empty,
                                        IFSC_Code = AccountDetails.IfsCode ?? string.Empty,
                                        CDA_PAO = common.pcda_pao ?? string.Empty,
                                        CDA_Account_No = common.pcda_AcctNo ?? string.Empty,
                                        Year_Of_Service = common.TotalService,
                                        Residual_Service = common.ResidualService,
                                        ApplicationType = common.ApplicationType == 1 ? hba.PropertyType : common.ApplicationType == 2 ? car.Veh_Loan_Type : pca.computer_Loan_Type,
                                        
                                        Salary_Slip_Month_Year = common.MonthlyPaySlip.ToString(),
                                        Basic_Salary = common.BasicPay,
                                        Rank_Grade_Pay = common.rank_gradePay,
                                        MSP = common.Msp,
                                        NPA_X_Pay = common.npax_Pay,
                                        DA = common.Da,
                                        MISC_Pay = common.MiscPay,
                                        PLI = common.Pli,
                                        AGIF = common.agif_Subs,
                                        Income_Tax_Monthly = common.IncomeTaxMonthly,
                                        DSOP_AFPP = common.dsop_afpp,
                                        MISC = common.misc_Deduction,
                                        Total = common.TotalCredit,
                                        Salary_After_Deduction = common.salary_After_Deductions,
                                        Pin_Code = AddressDetails.Code ?? string.Empty,
                                        E_Mail_Id = common.Email ?? string.Empty,

                                        ApplicationID = common.ApplicationId,
                                        CL_Pay = common.CI_Pay,
                                        EducationCess = common.EducationCess,
                                        LoanEMI_Outside = common.loanEMI_Outside,
                                        LoanEMI_AGIF = common.LoanEmi,
                                        LRA = common.Lra,
                                        PMHA = common.Pmha,
                                        StatusCode= common.StatusCode,
                                        ParentUnit = parentUnit != null ? parentUnit.UnitName : string.Empty,
                                        Dealer_Name = common.ApplicationType == 1 ? hba.PropertySeller : common.ApplicationType == 2?car.DealerName:pca.PCA_dealerName,
                                        Vehicle_Name = common.ApplicationType == 1 ? hba.PropertyAddress : common.ApplicationType == 2 ? car.ModelName : pca.PCA_modelName,
                                        Vehicle_Make = common.ApplicationType == 1 ? "" : common.ApplicationType == 2 ? car.CompanyName : pca.PCA_companyName,
                                        Total_Cost = common.ApplicationType == 1 ? hba.PropertyCost : common.ApplicationType == 2 ? car.VehicleCost : pca.computerCost,
                                        Amount_Applied_For_Loan = common.ApplicationType == 1 ? hba.HBA_Amount_Applied_For_Loan : common.ApplicationType == 2 ? car.CA_Amount_Applied_For_Loan : pca.PCA_Amount_Applied_For_Loan,
                                        No_Of_EMI_Applied = common.ApplicationType == 1 ? hba.HBA_EMI_Applied : common.ApplicationType == 2 ? car.CA_EMI_Applied : pca.PCA_EMI_Applied,
                                        VehType = common.ApplicationType == 2 ? car.VehTypeId:0, 

                                    }).ToList();

            dataTable = query.ToDataTable();
            return Task.FromResult(dataTable);
        }

        public async Task<bool> InsertStatusCounter(TrnStatusCounter trnStatusCounter)
        {
            if (trnStatusCounter == null)
                return false;

            await _context.TrnStatusCounter.AddAsync(trnStatusCounter);
            await SaveAsync(); // Make sure this saves changes
            return true;
        }

        public async Task<int> GetVehicleType(int applicationId, string formType)
        {
            if(formType == null || applicationId <= 0)
            {
                return 0;
            }
            int vehicleType = 0;
            if (formType == "CA")
            {
                  vehicleType = await _context.trnCar
                    .Where(c => c.ApplicationId == applicationId)
                    .Select(c => c.Veh_Loan_Type)
                    .FirstOrDefaultAsync();
            }
            
            return vehicleType;
        }
    }
}
