using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using DataTransferObject.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        public ClaimOnlineApplicationDL(ApplicationDbContext context, IArmyPrefixes iArmyPrefixes, IEducation education, IMarraige marraige, IProperty property, ISpecial special, IClaimDocumentUpload documentUpload) : base(context)
        {
            _context = context;
            _IArmyPrefixes = iArmyPrefixes;
            _Education = education;
            _Marraige = marraige;
            _Property = property;
            _Special = special;
            _DocumentUpload = documentUpload;
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

            // Check if the folder exists
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
                    else if (file.Name.Contains("SplWaiver"))
                    {
                        fileUpload.IsSplWaiverPdf = true;
                        fileUpload.SplWaiverPdf = fileName; // Update with the dynamic file name
                    }
                }
            }

            if (PurposeType == "ED")
            {
                var Eddetails= await _Education.GetByApplicationId(ApplicationId);
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
                fileUpload.Attach_PartIIOrderPdf = PRdetails.TotalExpenditureFilePdf;
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

                return true;
        }


    }
}
