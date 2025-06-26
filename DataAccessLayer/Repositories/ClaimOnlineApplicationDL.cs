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
        public ClaimOnlineApplicationDL(ApplicationDbContext context, IArmyPrefixes iArmyPrefixes, IEducation education, IMarraige marraige, IProperty property, ISpecial special) : base(context)
        {
            _context = context;
            _IArmyPrefixes = iArmyPrefixes;
            _Education = education;
            _Marraige = marraige;
            _Property = property;
            _Special = special;
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

            if(PurposeType== "SP")
            {
                SplWaiverModel splWaiver = new SplWaiverModel();
                splWaiver = model.SplWaiver;
                splWaiver.ApplicationId = commonDataModel.ApplicationId;
                await _Special.Add(model.SplWaiver);
            }
            else
            {
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
                    string fileName = $"{PurposeType}_{ArmyNo}_{ApplicationId}_{file.Name}{fileExtension}";
                    string outputFile = Path.Combine(folderPath, fileName);

                    using (var fileStream = new FileStream(outputFile, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }


                    if (file.Name.Equals(model.EducationDetails.AttachBonafideLetter.Name))
                    {
                        model.EducationDetails.IsAttachBonafideLetterPdf = true;
                    }
                    else if (file.Name.Equals(model.EducationDetails.AttachPartIIOrder.Name))
                    {
                        model.EducationDetails.IsAttachPartIIOrderPdf = true;
                    }

                    if (model.Marriageward != null)
                    {
                        if (file.Name.Equals(model.Marriageward.AttachInvitationcard.Name))
                        {
                            model.Marriageward.IsAttachInvitationcardPdf = true;
                        }
                        else if (file.Name.Equals(model.Marriageward.AttachPartIIOrder.Name))
                        {
                            model.Marriageward.IsAttachPartIIOrderPdf = true;
                        }

                    }
                    if (model.PropertyRenovation != null)
                    {
                        if (file.Name.Equals(model.PropertyRenovation.TotalExpenditureFile.Name))
                        {
                            model.PropertyRenovation.IsTotalExpenditureFilePdf = true;
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
            }
            


                return true;
        }
    }
}
