using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DataAccessLayer.Repositories
{
    public class ClaimOnlineApplicationDL : GenericRepositoryDL<ClaimCommonModel>, IClaimOnlineApplication
    {

        protected new readonly ApplicationDbContext _context;
        private readonly IArmyPrefixes _IArmyPrefixes;
        private readonly IEducation _Education;
        private readonly IMarraige _Marraige;
        private readonly IProperty _Property;
        private readonly ISpecial _Special;
        private readonly IClaimDocumentUpload _DocumentUpload;
        private readonly IOnlineApplication _onlineApplication;
        public ClaimOnlineApplicationDL(ApplicationDbContext context, IArmyPrefixes iArmyPrefixes, IEducation education, IMarraige marraige, IProperty property, ISpecial special, IClaimDocumentUpload documentUpload, IOnlineApplication onlineApplication) : base(context)
        {
            _context = context;
            _IArmyPrefixes = iArmyPrefixes;
            _Education = education;
            _Marraige = marraige;
            _Property = property;
            _Special = special;
            _DocumentUpload = documentUpload;
            _onlineApplication = onlineApplication;
        }

        public bool ValidateFileUpload(IFormFile file, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Check if file is null
            if (file == null)
            {
                errorMessage = "No file selected.";
                return false;
            }

            // Check if the file type is PDF
            if (file.ContentType != "application/pdf")
            {
                errorMessage = "Only PDF files are allowed.";
                return false;
            }

            // Check if the file size exceeds 1 MB (1 * 1024 * 1024 bytes)
            if (file.Length > 1 * 1024 * 1024)
            {
                errorMessage = "File size cannot exceed 1 MB.";
                return false;
            }

            return true;
        }

        public async Task<ClaimCommonDataOnlineResponse> GetApplicationDetailsByArmyNo(string armyNumber, string Prefix, string Suffix, int appType)
        {
            var existingUser = await (from app in _context.trnClaim
                                      join doc in _context.trnClaimDocumentUpload on app.ApplicationId equals doc.ApplicationId into docGroup
                                      from doc in docGroup.DefaultIfEmpty()
                                      where app.WithdrawPurpose == appType && (app.ArmyPrefix.ToString() + app.Number + app.Suffix) == (armyNumber + Prefix + Suffix)
                                      select new ClaimCommonDataOnlineResponse
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
                var ED = await _context.trnEducationDetails
                    .FirstOrDefaultAsync(c => c.ApplicationId == existingUser.ApplicationId);

                var MW = await _context.trnMarriageward
                    .FirstOrDefaultAsync(h => h.ApplicationId == existingUser.ApplicationId);

                var PC = await _context.trnPropertyRenovation
                    .FirstOrDefaultAsync(p => p.ApplicationId == existingUser.ApplicationId);
                
                var Sp= await _context.trnSplWaiver
                    .FirstOrDefaultAsync(s => s.ApplicationId == existingUser.ApplicationId);

                var documents = await _context.trnClaimDocumentUpload
                    .Where(d => d.ApplicationId == existingUser.ApplicationId)
                    .ToListAsync();


                if (ED != null)
                    _context.trnEducationDetails.Remove(ED);

                if (MW != null)
                    _context.trnMarriageward.Remove(MW);

                if (PC != null)
                    _context.trnPropertyRenovation.Remove(PC);

                if(Sp != null)
                    _context.trnSplWaiver.Remove(Sp);

                if (documents.Any())
                    _context.trnClaimDocumentUpload.RemoveRange(documents);

                var applicationEntity = await _context.trnClaim
                    .FirstOrDefaultAsync(a => a.ApplicationId == existingUser.ApplicationId);

                if (applicationEntity != null)
                    _context.trnClaim.Remove(applicationEntity);

                await _context.SaveChangesAsync();
                return true;

            }

            return false;
        }


        public async Task<bool> submitApplication(DTOClaimApplication model,string PurposeType ,int ApplicationId)
        {
            ClaimCommonModel commonDataModel = new ClaimCommonModel();
            MArmyPrefix mArmyPrefix = new MArmyPrefix();
            string ArmyNo = string.Empty;

                if (ApplicationId != 0)
                {
                    commonDataModel = _context.trnClaim.FirstOrDefault(c => c.ApplicationId == ApplicationId); ;
                    int id = commonDataModel.ArmyPrefix;
                    mArmyPrefix = await _IArmyPrefixes.Get(id);
                    ArmyNo = (mArmyPrefix.Prefix ?? "") + (commonDataModel.Number ?? "") + (commonDataModel.Suffix ?? "");
                    ArmyNo = ArmyNo.Trim();
                }

                var files = new List<IFormFile>();
                if (model.EducationDetails != null)
                {
                    if (model.EducationDetails.AttachBonafideLetter != null) files.Add(model.EducationDetails.AttachBonafideLetter);
                    if (model.EducationDetails.AttachPartIIOrder != null) files.Add(model.EducationDetails.AttachPartIIOrder);
                }
                else if (model.Marriageward != null)
                {
                    if (model.Marriageward.AttachInvitationcard != null) files.Add(model.Marriageward.AttachInvitationcard);
                    if (model.Marriageward.AttachPartIIOrder != null) files.Add(model.Marriageward.AttachPartIIOrder);
                }
                else if (model.PropertyRenovation != null)
                {
                    if (model.PropertyRenovation.TotalExpenditureFile != null) files.Add(model.PropertyRenovation.TotalExpenditureFile);
                }
                else if (model.SplWaiver != null)
                {
                    if (model.SplWaiver.OtherReasonPdf != null) files.Add(model.SplWaiver.OtherReasonPdf);
                }

            string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ClaimTempUploads");
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                string folderName = $"{PurposeType}_{ArmyNo}_{ApplicationId}";
                string folderPath = Path.Combine(tempFolder, folderName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                foreach (var file in files)
                {
                    string fileExtension = Path.GetExtension(file.FileName);
                    string originalFileName = file.Name;  // e.g., "EducationDetails.AttachBonafideletter"
                    string fileBaseName = originalFileName.Substring(originalFileName.IndexOf('.') + 1);
                    string fileName = $"{PurposeType}_{ArmyNo}_{ApplicationId}_{fileBaseName}{fileExtension}";
                    string outputFile = Path.Combine(folderPath, fileName);

                    using (var fileStream = new FileStream(outputFile, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    if (model.EducationDetails != null)
                    {
                        if (file.Name.Equals(model.EducationDetails.AttachBonafideLetter.Name))
                        {
                            model.EducationDetails.AttachBonafideLetterPdf = fileName;
                            model.EducationDetails.IsAttachBonafideLetterPdf = true;
                        }
                        else if (file.Name.Equals(model.EducationDetails.AttachPartIIOrder.Name))
                        {
                            model.EducationDetails.AttachPartIIOrderPdf = fileName;
                            model.EducationDetails.IsAttachPartIIOrderPdf = true;
                        }
                    }

                    if (model.Marriageward != null)
                    {
                        if (file.Name.Equals(model.Marriageward.AttachInvitationcard.Name))
                        {
                            model.Marriageward.AttachInvitationcardPdf = fileName;
                            model.Marriageward.IsAttachInvitationcardPdf = true;
                        }
                        else if (file.Name.Equals(model.Marriageward.AttachPartIIOrder.Name))
                        {
                            model.Marriageward.AttachPartIIOrderPdf = fileName;
                            model.Marriageward.IsAttachPartIIOrderPdf = true;
                        }

                    }
                    if (model.PropertyRenovation != null)
                    {
                        if (file.Name.Equals(model.PropertyRenovation.TotalExpenditureFile.Name))
                        {
                            model.PropertyRenovation.TotalExpenditureFilePdf = fileName;
                            model.PropertyRenovation.IsTotalExpenditureFilePdf = true;
                        }
                    }

                    if(model.SplWaiver!=null)
                    {
                        if (file.Name.Equals(model.SplWaiver.OtherReasonPdf.Name))
                        {
                            model.SplWaiver.OtherReasonsPdf = fileName;
                            model.SplWaiver.IsOtherReasonPdf = true;
                        }
                    }

                }

                if (model.EducationDetails != null)
                {
                    EducationDetailsModel educationDetails = new EducationDetailsModel();
                    educationDetails = model.EducationDetails;
                    educationDetails.ApplicationId = commonDataModel.ApplicationId;
                    await _Education.Add(model.EducationDetails);
                }
                else if (model.Marriageward != null)
                {
                    MarriagewardModel marriageward = new MarriagewardModel();
                    marriageward = model.Marriageward;
                    marriageward.ApplicationId = commonDataModel.ApplicationId;
                    await _Marraige.Add(model.Marriageward);
                }
                else if (model.PropertyRenovation != null)
                {
                    PropertyRenovationModel propertyRenovation = new PropertyRenovationModel();
                    propertyRenovation = model.PropertyRenovation;
                    propertyRenovation.ApplicationId = commonDataModel.ApplicationId;
                    await _Property.Add(model.PropertyRenovation);
                }

                else if (PurposeType == "SP")
                {
                    SplWaiverModel splWaiver = new SplWaiverModel();
                    splWaiver = model.SplWaiver;
                    splWaiver.ApplicationId = commonDataModel.ApplicationId;
                    await _Special.Add(model.SplWaiver);
                }
           



                return true;
        }


        public async Task<bool> UpdateApplicationStatus(int applicationId, int status)
        {
            var application = await _context.trnClaim.Where(i => i.ApplicationId == applicationId).SingleOrDefaultAsync();
            if (application == null)
            {
                return false; // Just exit the method if not found
            }

            application.StatusCode = status;
            _context.trnClaim.Update(application);
            await _context.SaveChangesAsync();

            return true;
        }

        public Task<string?> GetIOArmyNoAsync(int applicationId)
        {
            var application = _context.trnClaim
                .FirstOrDefault(i => i.ApplicationId == applicationId);

            if (application == null || string.IsNullOrWhiteSpace(application.IOArmyNo))
            {
                return Task.FromResult<string?>(null);
            }

            return Task.FromResult(application.IOArmyNo);
        }

        public async Task<UserMapping?> GetCoDetails(int applicationId)
        {
            // Step 1: Get the application by applicationId
            var application = await _context.trnClaim
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (application == null)
                return null;

            // Step 3: Get the UserMapping for the PresentUnit where IsPrimary and IsActive are true
            var userMapping = await _context.trnUserMappings
                .FirstOrDefaultAsync(m => m.UnitId == application.PresentUnit && m.IsPrimary);

            return userMapping;
        }

        public async Task<bool> AddFwdCO(TrnFwdCO trnFwdCO)
        {
            await _context.TrnFwdCO.AddAsync(trnFwdCO);
            await SaveAsync();
            return false;
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

        public async Task<bool> ProcessFileUploads(List<IFormFile> files, string PurposeType, int ApplicationId)
        {
            ClaimCommonModel commonDataModel = new ClaimCommonModel();
            MArmyPrefix mArmyPrefix = new MArmyPrefix();
            string ArmyNo = string.Empty;

            if (ApplicationId != 0)
            {
                commonDataModel = _context.trnClaim.FirstOrDefault(c => c.ApplicationId == ApplicationId);
                int id = commonDataModel.ArmyPrefix;
                mArmyPrefix = await _IArmyPrefixes.Get(id);
                ArmyNo = (mArmyPrefix.Prefix ?? "") + (commonDataModel.Number ?? "") + (commonDataModel.Suffix ?? "");
                ArmyNo = ArmyNo.Trim();
            }

            string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ClaimTempUploads");
            string folderName = $"{PurposeType}_{ArmyNo}_{ApplicationId}";
            string folderPath = Path.Combine(tempFolder, folderName);

          //  Check if the folder exists
            if (!Directory.Exists(folderPath))
            {
                // Folder not found, return false or handle as needed
                return false;
            }

            var fileUpload = new ClaimDocumentUpload();
            fileUpload.ApplicationId = ApplicationId;
            // Add PDFs to the folder
            foreach (var file in files)
            {
                if (file != null)
                {
                    // Generate a file name based on PurposeType, ArmyNo, ApplicationId, and the file name
                    string fileExtension = Path.GetExtension(file.FileName);
                    string fileName = $"{PurposeType}_{ArmyNo}_{ApplicationId}_{file.Name}{fileExtension}";
                    string outputFile = Path.Combine(folderPath, fileName);

                    // Save the file to the folder
                    using (var fileStream = new FileStream(outputFile, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    if (file.Name.Contains("CancelledCheque"))
                    {
                        fileUpload.IsCancelledChequePdf = true;
                        fileUpload.CancelledCheque = fileName; // Update with the dynamic file name
                    }
                    else if (file.Name.Contains("PaySlip"))
                    {
                        fileUpload.IsPaySlipPdf = true;
                        fileUpload.PaySlipPdf = fileName; // Update with the dynamic file name
                    }
                    else if (file.Name.Contains("Spdocus"))
                    {
                        fileUpload.IsSplWaiverPdf = true;
                        fileUpload.SplWaiverPdf = fileName; // Update with the dynamic file name
                    }
                    else if (file.Name.Contains("SeviceExtn"))
                    {
                        fileUpload.IsSeviceExtnPdf = true;
                        fileUpload.SeviceExtnPdf = fileName; // Update with the dynamic file name
                    }
                }
            }

            if (PurposeType == "ED")
            {
                var Eddetails = await _Education.GetByApplicationId(ApplicationId);
                fileUpload.AttachBonafideLetterPdf = Eddetails.AttachBonafideLetterPdf;
                fileUpload.IsAttachBonafideLetterPdf = Eddetails.IsAttachBonafideLetterPdf;
                fileUpload.AttachPartIIOrderPdf = Eddetails.AttachPartIIOrderPdf;
                fileUpload.IsAttachPartIIOrderPdf = Eddetails.IsAttachPartIIOrderPdf;
            }
            else if (PurposeType == "MW")
            {
                var MWdetails = await _Marraige.GetByApplicationId(ApplicationId);
                fileUpload.Attach_PartIIOrderPdf = MWdetails.AttachPartIIOrderPdf;
                fileUpload.IsAttach_PartIIOrderPdf = MWdetails.IsAttachPartIIOrderPdf;
                fileUpload.AttachInvitationcardPdf = MWdetails.AttachInvitationcardPdf;
                fileUpload.IsAttachInvitationcardPdf = MWdetails.IsAttachInvitationcardPdf;
            }
            else if (PurposeType == "PR")
            {
                // Process Property Renovation Details
                var PRdetails = await _Property.GetByApplicationId(ApplicationId);
                fileUpload.TotalExpenditureFile = PRdetails.TotalExpenditureFilePdf;
                fileUpload.IsTotalExpenditureFilePdf = PRdetails.IsTotalExpenditureFilePdf;
            }
            else if (PurposeType == "SP")
            {
                // Process Special Waiver Details
                var SPdetails = await _Special.GetByApplicationId(ApplicationId);
                fileUpload.OtherReasonsPdf = SPdetails.OtherReasonsPdf;
                fileUpload.IsOtherReasonPdf = SPdetails.IsOtherReasonPdf;
            }

            await _DocumentUpload.Add(fileUpload);


            await UpdateApplicationStatus(ApplicationId, 1);

            TrnStatusCounter trnStatusCounter = new TrnStatusCounter
            {
                StatusId = 1,
                ApplicationId = ApplicationId,
                ActionOn = DateTime.Now,
            };
            await _onlineApplication.InsertStatusCounter(trnStatusCounter);

            var IOArmyNo = await GetIOArmyNoAsync(ApplicationId);
            if (IOArmyNo == null)
            {
                var CoDetails = await GetCoDetails(ApplicationId);
                if (CoDetails != null)
                {
                    TrnFwdCO trnFwdCO = new TrnFwdCO
                    {
                        ApplicationId = ApplicationId,
                        ArmyNo = ArmyNo,
                        COUserId = CoDetails.UserId,
                        ProfileId = CoDetails.ProfileId,
                        CreatedOn = DateTime.Now,
                        Status = 1
                    };
                    await AddFwdCO(trnFwdCO);
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(IOArmyNo))
                {
                    var IoDetails = await GetUserDetails(IOArmyNo);
                    if (IoDetails != null)
                    {
                        TrnFwdCO trnFwdCO = new TrnFwdCO
                        {
                            ApplicationId = ApplicationId,
                            ArmyNo = ArmyNo,
                            COUserId = IoDetails.UserId,
                            ProfileId = IoDetails.ProfileId,
                            CreatedOn = DateTime.Now,
                            Status = 1
                        };
                        await AddFwdCO(trnFwdCO);
                    }

                }
            }

            return true;
        }


        public Task<DTOClaimCommonOnlineResponse> GetApplicationDetails(int applicationId)
        {
            DTOClaimCommonOnlineResponse data = new DTOClaimCommonOnlineResponse();

            var result = (from common in _context.trnClaim
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
                          join applicationType in _context.WithdrawalPurpose on common.WithdrawPurpose equals applicationType.Id
                          where common.ApplicationId == applicationId
                          select new ClaimCommonDataOnlineResponse
                          {
                              ParentUnit = parentUnit != null ? parentUnit.UnitName : string.Empty,
                              PresentUnit = presentUnit != null ? presentUnit.UnitName : string.Empty,
                              ApplicationId = common.ApplicationId,
                              ApplicationType = common.WithdrawPurpose,
                              //ApplicationTypeName = applicationType.ApplicationTypeName,
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
                              DateOfPromotion = common.DateOfPromotion,
                              DateOfRetirement = common.DateOfRetirement,
                              PanCardNo = common.PanCardNo ?? string.Empty,
                              MobileNo = common.MobileNo ?? string.Empty,
                              Email = common.Email ?? string.Empty,
                              EmailDomain = common.EmailDomain ?? string.Empty,
                              SalaryAcctNo = common.SalaryAcctNo ?? string.Empty,
                              ConfirmSalaryAcctNo = common.ConfirmSalaryAcctNo ?? string.Empty,
                              IfsCode = common.IfsCode ?? string.Empty,
                              NameOfBank = common.NameOfBank ?? string.Empty,
                              NameOfBankBranch = common.NameOfBankBranch ?? string.Empty,
                              pcda_pao = common.pcda_pao ?? string.Empty,
                              pcda_AcctNo = common.pcda_AcctNo ?? string.Empty,
                              CivilPostalAddress = common.CivilPostalAddress ?? string.Empty,
                              AmountwithdrwalRequired = common.AmountOfWithdrawalRequired ?? 0,
                              TotalService= common.TotalService ?? 0,
                              NoOfwithdrwal=common.Noofwithdrawal ?? string.Empty,

                              House_Building_Advance_Loan = common.House_Building_Advance_Loan ?? false,
                              
                              House_Repair_Advance_Loan = common.House_Repair_Advance_Loan ?? false,
                              
                              Conveyance_Advance_Loan= common.Conveyance_Advance_Loan ?? false,
                              
                              Computer_Advance_Loan= common.Computer_Advance_Loan ?? false,

                              House_Building_Date_of_Loan_taken = common.House_Building_Date_of_Loan_taken,
                              House_Building_Amount_Taken = common.House_Building_Amount_Taken ?? 0,
                              House_Building_Duration_of_Loan = common.House_Building_Duration_of_Loan ?? 0,

                              Conveyance_Amount_Taken = common.Conveyance_Amount_Taken ?? 0,
                              Conveyance_Date_of_Loan_taken= common.Conveyance_Date_of_Loan_taken,
                              Conveyance_Duration_of_Loan= common.Conveyance_Duration_of_Loan ?? 0,

                              House_Repair_Advance_Amount_Taken= common.House_Repair_Advance_Amount_Taken ?? 0,
                              House_Repair_Advance_Date_of_Loan_taken=common.House_Repair_Advance_Date_of_Loan_taken,
                              House_Repair_Advance_Duration_of_Loan= common.House_Repair_Advance_Duration_of_Loan ?? 0,

                              Computer_Amount_Taken = common.Computer_Amount_Taken ?? 0,
                              Computer_Date_of_Loan_taken= common.Computer_Date_of_Loan_taken,
                              Computer_Duration_of_Loan= common.Computer_Duration_of_Loan ?? 0,

                              Vill_Town = common.Vill_Town ?? string.Empty,
                              PostOffice = common.PostOffice ?? string.Empty,
                              Distt = common.Distt ?? string.Empty,
                              State = common.State ?? string.Empty,

                          }).FirstOrDefault();
            string formtype = string.Empty;
            if (result != null)
            {
                //if(result.ApplicationType==1)
                if (result.ApplicationType == 1)
                {
                    formtype = "ED";
                    var EDmodel = (from ED in _context.trnEducationDetails
                                    where ED.ApplicationId == applicationId
                                    select new DTOEducationDetailsResponse
                                    {
                                        ChildName = ED.ChildName,
                                        DateOfBirth = ED.DateOfBirth,
                                        DOPartIINo = ED.DOPartIINo,
                                        DoPartIIDate = ED.DoPartIIDate,
                                        CourseForWithdrawal = ED.CourseForWithdrawal,
                                        CollegeInstitution = ED.CollegeInstitution,
                                        TotalExpenditure = ED.TotalExpenditure
                                    }).FirstOrDefault();

                    data.OnlineApplicationResponse = result; // Assuming result is already defined

                    // Directly assign the DTO
                    data.EducationDetailsResponse = EDmodel;
                }

                else if (result.ApplicationType == 2)
                {
                    formtype = "MW";
                    var MWmodel = (from MW in _context.trnMarriageward
                                    where MW.ApplicationId == applicationId
                                    select new DTOMarraigeWardResponse
                                    {
                                        NameOfChild = MW.NameOfChild,
                                        DateOfBirth = MW.DateOfBirth,
                                        DOPartIINo = MW.DOPartIINo,
                                        DoPartIIDate = MW.DoPartIIDate,
                                        AgeOfWard = MW.AgeOfWard,
                                        DateofMarriage = MW.DateofMarriage,
                                    }).FirstOrDefault();

                    data.OnlineApplicationResponse = result;

                    data.MarraigeWardResponse = MWmodel;
                }

                else if (result.ApplicationType == 3)
                {
                    formtype = "PR";
                    var PRModal = (from PR in _context.trnPropertyRenovation
                                    where PR.ApplicationId == applicationId
                                    select new DTOPropertyRenovationResponse
                                    {
                                        PropertyHolderName = PR.PropertyHolderName,
                                        AddressOfProperty = PR.AddressOfProperty,
                                        EstimatedCost= PR.EstimatedCost,
                                    }).FirstOrDefault();

                    data.OnlineApplicationResponse = result; // Assuming result is already defined

                    // Directly assign the DTO
                    data.PropertyRenovationResponse = PRModal;
                }
                else if (result.ApplicationType == 4)
                {
                    formtype = "SP";
                    var SPModal = (from sp in _context.trnSplWaiver
                                    where sp.ApplicationId == applicationId
                                    select new DTOSplWaiverResponse
                                    {
                                       OtherReasons = sp.OtherReasons,

                                    }).FirstOrDefault();

                    data.OnlineApplicationResponse = result; // Assuming result is already defined

                    // Directly assign the DTO
                    data.SplWaiverResponse = SPModal;
                }

                //var DocumentModel = _context.trnDocumentUpload.FirstOrDefault(x => x.ApplicationId == applicationId);
                var DocumentModel = _context.trnClaimDocumentUpload.FirstOrDefault(x => x.ApplicationId == applicationId);

                if (DocumentModel != null)
                {
                    string directoryPath = Path.Combine("/ClaimTempUploads", $"{formtype}_{result.Number}_{applicationId}");
                    List<DTODocumentFileView> lstdoc = new List<DTODocumentFileView>();

                    if (DocumentModel.IsAttachBonafideLetterPdf)
                    {
                        DTODocumentFileView dTODocumentFileView = new DTODocumentFileView();
                        dTODocumentFileView.FileName = DocumentModel.AttachBonafideLetterPdf;
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }
                    if (DocumentModel.IsAttachPartIIOrderPdf)
                    {
                        DTODocumentFileView dTODocumentFileView = new DTODocumentFileView();
                        dTODocumentFileView.FileName = DocumentModel.AttachPartIIOrderPdf;
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }
                    if (DocumentModel.IsAttachInvitationcardPdf)
                    {
                        DTODocumentFileView dTODocumentFileView = new DTODocumentFileView();
                        dTODocumentFileView.FileName = DocumentModel.AttachInvitationcardPdf;
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }
                    if (DocumentModel.IsAttach_PartIIOrderPdf)
                    {
                        DTODocumentFileView dTODocumentFileView = new DTODocumentFileView();
                        dTODocumentFileView.FileName = DocumentModel.Attach_PartIIOrderPdf;
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }
                    if (DocumentModel.IsTotalExpenditureFilePdf)
                    {
                        DTODocumentFileView dTODocumentFileView = new DTODocumentFileView();
                        dTODocumentFileView.FileName = DocumentModel.TotalExpenditureFile;
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }
                    if (DocumentModel.IsCancelledChequePdf)
                    {
                        DTODocumentFileView dTODocumentFileView = new DTODocumentFileView();
                        dTODocumentFileView.FileName = DocumentModel.CancelledCheque;
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }
                    if (DocumentModel.IsPaySlipPdf)
                    {
                        DTODocumentFileView dTODocumentFileView = new DTODocumentFileView();
                        dTODocumentFileView.FileName = DocumentModel.PaySlipPdf;
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }
                    if (DocumentModel.IsSplWaiverPdf)
                    {
                        DTODocumentFileView dTODocumentFileView = new DTODocumentFileView();
                        dTODocumentFileView.FileName = DocumentModel.SplWaiverPdf;
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }
                   if(DocumentModel.IsSeviceExtnPdf)
                    {
                        DTODocumentFileView dTODocumentFileView = new DTODocumentFileView();
                        dTODocumentFileView.FileName = DocumentModel.SeviceExtnPdf;
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }
                   if(DocumentModel.IsOtherReasonPdf)
                    {
                        DTODocumentFileView dTODocumentFileView = new DTODocumentFileView();
                        dTODocumentFileView.FileName = DocumentModel.OtherReasonsPdf;
                        dTODocumentFileView.FilePath = directoryPath;
                        lstdoc.Add(dTODocumentFileView);
                    }

                    data.Documents = lstdoc; // Assign the list of documents to the response object
                    // Get all files in the directory

                }




            }

            return Task.FromResult(data);
        }

        public async Task<string> GetCOName(int mappingId)
        {
            // Get the UserMapping by MappingId
            var userMapping = await _context.trnUserMappings.FirstOrDefaultAsync(m => m.MappingId == mappingId);


            if (userMapping == null)
                return string.Empty;

            // Get the UserProfile by ProfileId from UserMapping
            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.ProfileId == userMapping.MappingId);
            if (userProfile == null)
                return string.Empty;

            // Get the RankName from MRanks using rank id from UserProfile
            var rank = await _context.MRanks.FirstOrDefaultAsync(r => r.RankId == userProfile.rank);
            string rankName = rank != null ? rank.RankName : string.Empty;

            // Concatenate rankName and userName
            return $"{rankName} {userProfile.userName}".Trim();
        }

        public async Task<bool> UpdateMergePdfStatus(int applicationId, bool status)
        {
            var application = await _context.trnClaim.Where(i => i.ApplicationId == applicationId).SingleOrDefaultAsync();
            if (application == null)
            {
                return false; // Just exit the method if not found
            }

            application.IsMergePdf = status;
            _context.trnClaim.Update(application);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CheckExtensionofservice(int applicationid)
        {
            // Fetch the application record by applicationid
            var application = await _context.trnClaim
                .Where(a => a.ApplicationId == applicationid).FirstOrDefaultAsync();

            if (application == null)
                return false;

            if (string.IsNullOrEmpty(application.ExtnOfService))
                return false;
            else if (application.ExtnOfService == "Yes")
                return true;
            else if (application.ExtnOfService == "No")
                return false;
            else
                return false;

        }


        public Task<DTOClaimCommonOnlineApplicationResponseList> GetApplicationDetailsForExport(DTOExportRequest dTOExport)
        {
            DTOClaimCommonOnlineApplicationResponseList data = new DTOClaimCommonOnlineApplicationResponseList();

            var result = (from common in _context.trnClaim
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
                          join applicationType in _context.WithdrawalPurpose on common.WithdrawPurpose equals applicationType.Id

                          where dTOExport.Id.Contains(common.ApplicationId)
                          select new ClaimCommonDataOnlineResponse
                          {
                              ParentUnit = parentUnit != null ? parentUnit.UnitName : string.Empty,
                              PresentUnit = presentUnit != null ? presentUnit.UnitName : string.Empty,
                              ApplicationId = common.ApplicationId,
                              ApplicationType = common.WithdrawPurpose,
                              //ApplicationTypeAbbr = applicationType.ApplicationTypeAbbr ?? string.Empty,
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
                              DateOfPromotion = common.DateOfPromotion,
                              DateOfRetirement = common.DateOfRetirement,
                              PanCardNo = common.PanCardNo ?? string.Empty,
                              MobileNo = common.MobileNo ?? string.Empty,
                              Email = common.Email ?? string.Empty,
                              SalaryAcctNo = common.SalaryAcctNo ?? string.Empty,
                              IfsCode = common.IfsCode ?? string.Empty,
                              NameOfBank = common.NameOfBank ?? string.Empty,
                              NameOfBankBranch = common.NameOfBankBranch ?? string.Empty,
                              pcda_pao = common.pcda_pao ?? string.Empty,
                              pcda_AcctNo = common.pcda_AcctNo ?? string.Empty,
                              
                              House_Building_Advance_Loan = common.House_Building_Advance_Loan ?? false,

                              House_Repair_Advance_Loan = common.Computer_Advance_Loan ?? false,

                              Conveyance_Advance_Loan = common.Conveyance_Advance_Loan ?? false,

                              Computer_Advance_Loan = common.Computer_Advance_Loan ?? false,

                              House_Building_Date_of_Loan_taken = common.House_Building_Date_of_Loan_taken,
                              House_Building_Amount_Taken = common.House_Building_Amount_Taken ?? 0,
                              House_Building_Duration_of_Loan = common.House_Building_Duration_of_Loan ?? 0,

                              Conveyance_Amount_Taken = common.Conveyance_Amount_Taken ?? 0,
                              Conveyance_Date_of_Loan_taken = common.Conveyance_Date_of_Loan_taken,
                              Conveyance_Duration_of_Loan = common.Conveyance_Duration_of_Loan ?? 0,

                              House_Repair_Advance_Amount_Taken = common.House_Repair_Advance_Amount_Taken ?? 0,
                              House_Repair_Advance_Date_of_Loan_taken = common.House_Repair_Advance_Date_of_Loan_taken,
                              House_Repair_Advance_Duration_of_Loan = common.House_Repair_Advance_Duration_of_Loan ?? 0,

                              Computer_Amount_Taken = common.Computer_Amount_Taken ?? 0,
                              Computer_Date_of_Loan_taken = common.Computer_Date_of_Loan_taken,
                              Computer_Duration_of_Loan = common.Computer_Duration_of_Loan ?? 0,

                              Vill_Town = common.Vill_Town ?? string.Empty,
                              PostOffice = common.PostOffice ?? string.Empty,
                              Distt = common.Distt ?? string.Empty,
                              State = common.State ?? string.Empty,
                          }).ToListAsync();
            data.OnlineApplicationResponse = result.Result; // Assuming result is already defined

            return Task.FromResult(data);
        }

        public Task<DataTable> GetApplicationDetailsForExcel(DTOExportRequest dTOExport)
        {
            DataTable dataTable = new DataTable();
            var query = (from common in _context.trnClaim
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
                         join applicationType in _context.WithdrawalPurpose on common.WithdrawPurpose equals applicationType.Id

                         join ED in _context.trnEducationDetails on common.ApplicationId equals ED.ApplicationId into EDGroup
                         from ED in EDGroup.DefaultIfEmpty()

                         join MW in _context.trnMarriageward on common.ApplicationId equals MW.ApplicationId into MWGroup
                         from MW in MWGroup.DefaultIfEmpty()
                         
                         join PR in _context.trnPropertyRenovation on common.ApplicationId equals PR.ApplicationId into PRGroup
                         from PR in PRGroup.DefaultIfEmpty()

                         join SP in _context.trnSplWaiver on common.ApplicationId equals SP.ApplicationId into SPGroup
                         from SP in SPGroup.DefaultIfEmpty()

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

                             Promotion_Date = common.DateOfPromotion,
                             Retirement_Date = common.DateOfRetirement,
                             PANNo = common.PanCardNo ?? string.Empty,
                             Mobile_No = common.MobileNo ?? string.Empty,
                             Payee_Account_No = common.SalaryAcctNo ?? string.Empty,
                             IFSC_Code = common.IfsCode ?? string.Empty,
                             CDA_PAO = common.pcda_pao ?? string.Empty,
                             CDA_Account_No = common.pcda_AcctNo ?? string.Empty,
                             Year_Of_Service = common.TotalService,
                             Residual_Service = common.ResidualService,
                             //ApplicationType = common.ApplicationType == 1 ? hba.PropertyType : common.ApplicationType == 2 ? car.Veh_Loan_Type : pca.computer_Loan_Type,

                             E_Mail_Id = common.Email ?? string.Empty,


                             ApplicationID = common.ApplicationId,

                             StatusCode = common.StatusCode,
                             ParentUnit = parentUnit != null ? parentUnit.UnitName : string.Empty,
                          


                         }).ToList();

            dataTable = query.ToDataTable();
            return Task.FromResult(dataTable);
        }

    }
}
