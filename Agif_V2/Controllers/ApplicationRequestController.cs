using Agif_V2.Helpers;
using ClosedXML.Excel;
using DataAccessLayer.Interfaces;
using DataTransferObject.Helpers;
using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
//using OfficeOpenXml;
//using OfficeOpenXml.Style;
using System;
using System.Data;
using System.Drawing;
using System.IO.Compression;
namespace Agif_V2.Controllers
{
    public class ApplicationRequestController : Controller
    {
        private readonly IUsersApplications _userApplication;
        private readonly IOnlineApplication _onlineApplication;
        private readonly IApplication _application;
        private readonly IUserProfile _userProfile;
        private readonly IClaimOnlineApplication _IClaimonlineApplication1;

        public ApplicationRequestController(IUsersApplications usersApplications, IOnlineApplication _onlineApplication, IApplication _application,IUserProfile _userProfile,IClaimOnlineApplication claimOnlineApplication)
        {
            _userApplication = usersApplications;
            this._onlineApplication = _onlineApplication;
            this._application = _application;            
            this._userProfile = _userProfile;
            this._IClaimonlineApplication1 = claimOnlineApplication;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UserApplicationList(int status)
        {
            ViewBag.Status = status;
            SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
            if (dTOTempSession == null || dTOTempSession.ProfileId <= 0)
            {
                return Unauthorized("Session expired or invalid user session.");
            }
            ViewBag.ArmyNo = dTOTempSession.ArmyNo;
            return View(dTOTempSession);
        }
        public async Task<IActionResult> EditUser()
        {
            var sessionUser = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
            SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");

            DTOUserProfileResponse dTOUserProfileResponse = new DTOUserProfileResponse();

            if (dTOTempSession != null)
              dTOUserProfileResponse = await _userProfile.GetUserAllDetails(sessionUser.UserName);

            if (dTOUserProfileResponse != null)
            {
                // Map properties as needed. Example:
                dTOTempSession.ArmyNo = dTOUserProfileResponse.ArmyNo;
                dTOTempSession.MobileNo = dTOUserProfileResponse.MobileNo;
                dTOTempSession.ProfileName = dTOUserProfileResponse.ProfileName;
                dTOTempSession.UserName = dTOUserProfileResponse.DomainId;
                dTOTempSession.EmailId = dTOUserProfileResponse.EmailId;
                dTOTempSession.RankId = dTOUserProfileResponse.RankId;
                dTOTempSession.RegtId = dTOUserProfileResponse.RegtId;
                dTOTempSession.ApptId = dTOUserProfileResponse.ApptId;
                dTOTempSession.UnitId = dTOUserProfileResponse.UnitId;
                dTOTempSession.name = dTOUserProfileResponse.username;
                dTOTempSession.DteFmn = dTOUserProfileResponse.IsFmn;
            }

            return View(dTOTempSession);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(SessionUserDTO sessionUserDTO)
        {
            var result= await _userApplication.UpdateUserDetails(sessionUserDTO);
            if (result)
            {
                // Update session with new user details
                //Helpers.SessionExtensions.SetObject(HttpContext.Session, "User", sessionUserDTO);
                TempData["ProfileMessage"] = "Your Profile is Updated Successfully.";
                return RedirectToAction("EditUser", "ApplicationRequest");
            }
            else
            {
                ModelState.AddModelError("", "Failed to update user details. Please try again.");
            }
            return View();
        }

        public async Task<IActionResult> GetUsersApplicationList(DTODataTableRequest request, int status)
        {

            SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
            if (dTOTempSession == null || dTOTempSession.MappingId <= 0)
            {
                return Unauthorized("Session expired or invalid user session.");
            }

            var queryableData = await _userApplication.GetUsersApplication(dTOTempSession.MappingId, status);

            var totalRecords = queryableData.Count();

            var query = queryableData.AsQueryable();

            if (!string.IsNullOrEmpty(request.searchValue))
            {
                string searchValue = request.searchValue.ToLower();
                query = query.Where(x =>
                    x.Name.ToLower().Contains(searchValue) ||
                    x.ArmyNo.ToLower().Contains(searchValue) ||
                    x.DateOfBirth.ToLower().Contains(searchValue) ||
                    x.AppliedDate.ToLower().Contains(searchValue)
                );
            }

            var filteredRecords = query.Count();

            if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
            {
                bool ascending = request.sortDirection.ToLower() == "asc";

                query = request.sortColumn.ToLower() switch
                {
                    "name" => ascending ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name),
                    "armyno" => ascending ? query.OrderBy(x => x.ArmyNo) : query.OrderByDescending(x => x.ArmyNo),
                    "dateofbirth" => ascending ? query.OrderBy(x => x.DateOfBirth) : query.OrderByDescending(x => x.DateOfBirth),
                    "applieddate" => ascending ? query.OrderBy(x => x.AppliedDate) : query.OrderByDescending(x => x.AppliedDate),
                    _ => query // Default: no sorting if column not recognized
                };
            }

            // Paginate the result
            var paginatedData = query.Skip(request.Start).Take(request.Length).ToList();

            var responseData = new DTODataTablesResponse<DTOGetApplResponse>
            {
                draw = request.Draw,
                recordsTotal = totalRecords,
                recordsFiltered = filteredRecords,
                data = paginatedData
            };

            return Json(responseData);

        }

        public async Task<IActionResult> GetMaturityUsersApplicationList(DTODataTableRequest request, int status)
        {

            SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
            if (dTOTempSession == null || dTOTempSession.MappingId <= 0)
            {
                return Unauthorized("Session expired or invalid user session.");
            }

            var queryableData = await _userApplication.GetMaturityUsersApplication(dTOTempSession.MappingId, status);

            var totalRecords = queryableData.Count();

            var query = queryableData.AsQueryable();

            if (!string.IsNullOrEmpty(request.searchValue))
            {
                string searchValue = request.searchValue.ToLower();
                query = query.Where(x =>
                    x.Name.ToLower().Contains(searchValue) ||
                    x.ArmyNo.ToLower().Contains(searchValue) ||
                    x.DateOfBirth.ToLower().Contains(searchValue) ||
                    x.AppliedDate.ToLower().Contains(searchValue)
                );
            }

            var filteredRecords = query.Count();

            if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
            {
                bool ascending = request.sortDirection.ToLower() == "asc";

                query = request.sortColumn.ToLower() switch
                {
                    "name" => ascending ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name),
                    "armyno" => ascending ? query.OrderBy(x => x.ArmyNo) : query.OrderByDescending(x => x.ArmyNo),
                    "dateofbirth" => ascending ? query.OrderBy(x => x.DateOfBirth) : query.OrderByDescending(x => x.DateOfBirth),
                    "applieddate" => ascending ? query.OrderBy(x => x.AppliedDate) : query.OrderByDescending(x => x.AppliedDate),
                    _ => query // Default: no sorting if column not recognized
                };
            }

            // Paginate the result
            var paginatedData = query.Skip(request.Start).Take(request.Length).ToList();

            var responseData = new DTODataTablesResponse<DTOGetApplResponse>
            {
                draw = request.Draw,
                recordsTotal = totalRecords,
                recordsFiltered = filteredRecords,
                data = paginatedData
            };

            return Json(responseData);

        }

        public async Task<IActionResult> ViewDetails(int applicationId)
        {
            return View();
        }

        public async Task<string> DataDigitalXmlSign(int applicationId)
        {
            var data = SignDocument(applicationId);
            var jsonObject = new
            {
                applicationId = applicationId,
                ApplicantName = data.Result.ApplicantName,
                ArmyNo = data.Result.ArmyNo,
                ApplicationType = data.Result.ApplicationType,
                Unit = data.Result.UnitName,
                Rank = data.Result.RankName,
                DateOfCommission = data.Result.DateOfCommision,
                PanCard = data.Result.PAN_No,
                Account_No = data.Result.AccountNo
            };
            string jsonData = JsonConvert.SerializeObject(jsonObject);
            var xml = JsonConvert.DeserializeXNode(jsonData, "Root");
            return xml.ToString();
        }
        public async Task<string> ClaimDataDigitalXmlSign(int applicationId)
        {
            var data = ClaimSignDocument(applicationId);
            var jsonObject = new
            {
                applicationId = applicationId,
                ApplicantName = data.Result.ApplicantName,
                ArmyNo = data.Result.ArmyNo,
                ApplicationType = data.Result.ApplicationType,
                Unit = data.Result.UnitName,
                Rank = data.Result.RankName,
                DateOfCommission = data.Result.DateOfCommision,
                PanCard = data.Result.PAN_No,
                Account_No = data.Result.AccountNo
            };
            string jsonData = JsonConvert.SerializeObject(jsonObject);
            var xml = JsonConvert.DeserializeXNode(jsonData, "Root");
            return xml.ToString();
        }


        public async Task<DTODigitalSignDataResponse?> ClaimSignDocument(int applicationId)
        {
            DTOClaimCommonOnlineResponse data = await _IClaimonlineApplication1.GetApplicationDetails(applicationId);
            DTODigitalSignDataResponse digitalSignDTO = new DTODigitalSignDataResponse();

            if (data.OnlineApplicationResponse != null)
            {
                var onlineResponse = data.OnlineApplicationResponse;

                digitalSignDTO.ApplicationId = onlineResponse.ApplicationId;
                digitalSignDTO.ArmyNo = onlineResponse.Number ?? string.Empty;
                digitalSignDTO.ApplicantName = onlineResponse.ApplicantName ?? string.Empty;
                digitalSignDTO.PCDA_PAO = onlineResponse.pcda_pao ?? string.Empty;
                digitalSignDTO.Date_Of_Birth = onlineResponse.DateOfBirth?.ToString() ?? string.Empty;
                digitalSignDTO.Retirement_Date = onlineResponse.DateOfRetirement?.ToString() ?? string.Empty;
                digitalSignDTO.Mobile_No = onlineResponse.MobileNo ?? string.Empty;
                digitalSignDTO.ApplType = onlineResponse.ApplicationType;
                digitalSignDTO.DateOfCommision = onlineResponse.DateOfCommission?.ToString() ?? string.Empty;
                digitalSignDTO.AccountNo = onlineResponse.SalaryAcctNo ?? string.Empty;
                digitalSignDTO.RankName = onlineResponse.DdlRank ?? string.Empty;
                digitalSignDTO.UnitName = onlineResponse.PresentUnit ?? string.Empty;
                digitalSignDTO.PAN_No = onlineResponse.PanCardNo ?? string.Empty;

                return digitalSignDTO;
            }
            return null;
        }




        public async Task<DTODigitalSignDataResponse?> SignDocument(int applicationId)
        {
            DTOCommonOnlineApplicationResponse data = await _onlineApplication.GetApplicationDetails(applicationId);
            DTODigitalSignDataResponse digitalSignDTO = new DTODigitalSignDataResponse();

            if (data.OnlineApplicationResponse != null)
            {
                var onlineResponse = data.OnlineApplicationResponse;

                digitalSignDTO.ApplicationId = onlineResponse.ApplicationId;
                digitalSignDTO.ArmyNo = onlineResponse.Number ?? string.Empty;
                digitalSignDTO.ApplicantName = onlineResponse.ApplicantName ?? string.Empty;
                digitalSignDTO.PCDA_PAO = onlineResponse.pcda_pao ?? string.Empty;
                digitalSignDTO.Date_Of_Birth = onlineResponse.DateOfBirth?.ToString() ?? string.Empty;
                digitalSignDTO.Retirement_Date = onlineResponse.DateOfRetirement?.ToString() ?? string.Empty;
                digitalSignDTO.Mobile_No = onlineResponse.MobileNo ?? string.Empty;
                digitalSignDTO.ApplType = onlineResponse.ApplicationType;
                digitalSignDTO.DateOfCommision = onlineResponse.DateOfCommission?.ToString() ?? string.Empty;
                digitalSignDTO.AccountNo = onlineResponse.SalaryAcctNo ?? string.Empty;
                digitalSignDTO.RankName = onlineResponse.DdlRank ?? string.Empty;
                digitalSignDTO.UnitName = onlineResponse.PresentUnit ?? string.Empty;
                digitalSignDTO.PAN_No = onlineResponse.PanCardNo ?? string.Empty;

                return digitalSignDTO;
            }
            return null;
        }

        public async Task SaveXML(int applId, string xmlResString, string remarks)
        {
            try
            {
                DTOCommonOnlineApplicationResponse data = await _onlineApplication.GetApplicationDetails(applId);

                var dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
                if (dTOTempSession == null)
                    throw new Exception("Session expired or invalid user context.");



                var digitalSignRecords = new DigitalSignRecords
                {
                    ApplId = applId,
                    XMLSignResponse = xmlResString,
                    SignOn = DateTime.Now,
                    Remarks = remarks,
                    IsSign = true,
                    DomainId = dTOTempSession.DomainId,
                    ArmyNo = dTOTempSession.ArmyNo,
                    RankName = dTOTempSession.RankName
                };

                await _onlineApplication.UpdateApplicationStatus(applId, 2);

                TrnStatusCounter trnStatusCounter = new TrnStatusCounter
                {
                    StatusId = 2,
                    ApplicationId = applId,
                    ActionOn = DateTime.Now,
                };
                await _onlineApplication.InsertStatusCounter(trnStatusCounter);

                await _application.Add(digitalSignRecords);

                DTOUserProfileResponse adminDetails = await _userProfile.GetAdminDetails();
                var TrnFwd = new TrnFwd
                {
                    ApplicationId = applId,
                    FromUserId = dTOTempSession.UserId,
                    FromProfileId = dTOTempSession.ProfileId,
                    ToUserId = adminDetails.UserId,
                    ToProfileId = adminDetails.ProfileId,
                    CreatedOn = DateTime.Now
                };

                await _userProfile.SaveTrnFwdRecords(TrnFwd);// Save TrnFwd to database


            }
            catch (Exception ex)
            {
                // Log the error or return appropriate response
                Console.WriteLine($"Error in SaveXML: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public async Task SaveClaimXML(int applId, string xmlResString, string remarks)
        {
            try
            {
            DTOClaimCommonOnlineResponse data = await _IClaimonlineApplication1.GetApplicationDetails(applId);

                var dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
                if (dTOTempSession == null)
                    throw new Exception("Session expired or invalid user context.");



                var digitalSignRecords = new DigitalSignRecords
                {
                    ApplId = applId,
                    XMLSignResponse = xmlResString,
                    SignOn = DateTime.Now,
                    Remarks = remarks,
                    IsSign = true,
                    DomainId = dTOTempSession.DomainId,
                    ArmyNo = dTOTempSession.ArmyNo,
                    RankName = dTOTempSession.RankName
                };

                await _IClaimonlineApplication1.UpdateApplicationStatus(applId, 2);


                TrnStatusCounter trnStatusCounter = new TrnStatusCounter
                {
                    StatusId = 2,
                    ApplicationId = applId,
                    ActionOn = DateTime.Now,
                };
                await _onlineApplication.InsertStatusCounter(trnStatusCounter);

                await _application.Add(digitalSignRecords);

                DTOUserProfileResponse adminDetails = await _userProfile.GetAdminDetails();
                var TrnFwd = new TrnFwd
                {
                    ApplicationId = applId,
                    FromUserId = dTOTempSession.UserId,
                    FromProfileId = dTOTempSession.ProfileId,
                    ToUserId = adminDetails.UserId,
                    ToProfileId = adminDetails.ProfileId,
                    CreatedOn = DateTime.Now
                };

                await _userProfile.SaveTrnFwdRecords(TrnFwd);// Save TrnFwd to database


            }
            catch (Exception ex)
            {
                // Log the error or return appropriate response
                Console.WriteLine($"Error in SaveXML: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }
        public async Task<JsonResult> RejectXML(int applId, string rem)
        {
            var digitalSignRecords = new DigitalSignRecords
            {
                ApplId = applId,
                SignOn = DateTime.Now,
                Remarks = rem,
                IsSign = false,
                IsRejectced = true,
            };
            await _application.Add(digitalSignRecords);
            await _onlineApplication.UpdateApplicationStatus(applId, 3);
            TrnStatusCounter trnStatusCounter = new TrnStatusCounter
            {
                StatusId = 3,
                ApplicationId = applId,
                ActionOn = DateTime.Now,
            };
            await _onlineApplication.InsertStatusCounter(trnStatusCounter);
            return Json(new { success = true, message = "Application rejected." });

        }

        public async Task<JsonResult> ClaimRejectXML(int applId, string rem)
        {
            var digitalSignRecords = new DigitalSignRecords
            {
                ApplId = applId,
                SignOn = DateTime.Now,
                Remarks = rem,
                IsSign = false,
                IsRejectced = true,
            };
            await _application.Add(digitalSignRecords);
            await _IClaimonlineApplication1.UpdateApplicationStatus(applId, 3);
            //await _onlineApplicationController.MergePdf(applId, true, false);
            TrnStatusCounter trnStatusCounter = new TrnStatusCounter
            {
                StatusId = 3,
                ApplicationId = applId,
                ActionOn = DateTime.Now,
            };
            await _onlineApplication.InsertStatusCounter(trnStatusCounter);

            return Json(new { success = true, message = "Application rejected." });
        }
        public async Task<IActionResult> UsersApplicationListAdmin(string status)
        {
            ViewBag.Status = status;
            SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
            if (dTOTempSession == null || dTOTempSession.ProfileId <= 0)
            {
                return Unauthorized("Session expired or invalid user session.");
            }
            ViewBag.ArmyNo = dTOTempSession.ArmyNo;
            return View(dTOTempSession);
        }

        public async Task<IActionResult> ClaimApplicationListAdmin(string status)
        {
            ViewBag.Status = status;
            SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
            if (dTOTempSession == null || dTOTempSession.ProfileId <= 0)
            {
                return Unauthorized("Session expired or invalid user session.");
            }
            ViewBag.ArmyNo = dTOTempSession.ArmyNo;
            return View(dTOTempSession);
        }


        public async Task<IActionResult> GetUsersApplicationListToAdmin(DTODataTableRequest request, int status)
        {
            try
            {
                var queryableData = await _userApplication.GetUsersApplicationForAdmin(status);
                var totalRecords = queryableData.Count();
                var query = queryableData.AsQueryable();

                // Apply search filter - fixed to match actual DTO properties
                if (!string.IsNullOrEmpty(request.searchValue))
                {
                    string searchValue = request.searchValue.ToLower();
                    query = query.Where(x =>
                        (x.Name != null && x.Name.ToLower().Contains(searchValue)) ||
                        (x.ArmyNo != null && x.ArmyNo.ToLower().Contains(searchValue)) ||
                        (x.RegtCorps != null && x.RegtCorps.ToLower().Contains(searchValue)) ||
                        (x.PresentUnit != null && x.PresentUnit.ToLower().Contains(searchValue)) ||
                        (x.AppliedDate != null && x.AppliedDate.ToLower().Contains(searchValue))
                    );
                }

                var filteredRecords = query.Count();

                // Apply sorting - fixed to match JavaScript column names
                if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                {
                    bool ascending = string.Equals(request.sortDirection, "asc", StringComparison.OrdinalIgnoreCase); // Case-insensitive comparison for sort direction
                    query = string.Equals(request.sortColumn, "name", StringComparison.OrdinalIgnoreCase) ?
                        (ascending ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name)) :
                    string.Equals(request.sortColumn, "armyno", StringComparison.OrdinalIgnoreCase) ?
                        (ascending ? query.OrderBy(x => x.ArmyNo) : query.OrderByDescending(x => x.ArmyNo)) :
                    string.Equals(request.sortColumn, "regtname", StringComparison.OrdinalIgnoreCase) ?
                        (ascending ? query.OrderBy(x => x.RegtCorps) : query.OrderByDescending(x => x.RegtCorps)) :
                    string.Equals(request.sortColumn, "presentunit", StringComparison.OrdinalIgnoreCase) ?
                        (ascending ? query.OrderBy(x => x.PresentUnit) : query.OrderByDescending(x => x.PresentUnit)) :
                    string.Equals(request.sortColumn, "applieddate", StringComparison.OrdinalIgnoreCase) ?
                        (ascending ? query.OrderBy(x => x.AppliedDate) : query.OrderByDescending(x => x.AppliedDate)) :
                        query.OrderByDescending(x => x.UpdatedOn); // Default sorting
                }
                else
                {
                    // Default sorting by UpdatedOn descending
                    query = query.OrderByDescending(x => x.UpdatedOn);
                }

                // Paginate the result
                var paginatedData = query.Skip(request.Start).Take(request.Length).ToList();

                var responseData = new DTODataTablesResponse<DTOGetApplResponse>
                {
                    draw = request.Draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = paginatedData
                };

                return Json(responseData);
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { error = "An error occurred while loading data: " + ex.Message });
            }
        }
        public async Task<IActionResult> GetClaimUsersApplicationListToAdmin(DTODataTableRequest request, int status)
        {
            try
            {
                var queryableData = await _userApplication.GetClaimUsersApplicationForAdmin(status);
                var totalRecords = queryableData.Count();
                var query = queryableData.AsQueryable();

                // Apply search filter - fixed to match actual DTO properties
                if (!string.IsNullOrEmpty(request.searchValue))
                {
                    string searchValue = request.searchValue.ToLower();
                    query = query.Where(x =>
                        (x.Name != null && x.Name.ToLower().Contains(searchValue)) ||
                        (x.ArmyNo != null && x.ArmyNo.ToLower().Contains(searchValue)) ||
                        (x.RegtCorps != null && x.RegtCorps.ToLower().Contains(searchValue)) ||
                        (x.PresentUnit != null && x.PresentUnit.ToLower().Contains(searchValue)) ||
                        (x.AppliedDate != null && x.AppliedDate.ToLower().Contains(searchValue))
                    );
                }

                var filteredRecords = query.Count();

                // Apply sorting - fixed to match JavaScript column names
                if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                {
                    bool ascending = request.sortDirection.ToLower() == "asc";
                    query = request.sortColumn.ToLower() switch
                    {
                        "name" => ascending ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name),
                        "armyno" => ascending ? query.OrderBy(x => x.ArmyNo) : query.OrderByDescending(x => x.ArmyNo),
                        "regtname" => ascending ? query.OrderBy(x => x.RegtCorps) : query.OrderByDescending(x => x.RegtCorps),
                        "presentunit" => ascending ? query.OrderBy(x => x.PresentUnit) : query.OrderByDescending(x => x.PresentUnit),
                        "applieddate" => ascending ? query.OrderBy(x => x.AppliedDate) : query.OrderByDescending(x => x.AppliedDate),
                        _ => query.OrderByDescending(x => x.UpdatedOn) // Default sorting
                    };
                }
                else
                {
                    // Default sorting by UpdatedOn descending
                    query = query.OrderByDescending(x => x.UpdatedOn);
                }

                // Paginate the result
                var paginatedData = query.Skip(request.Start).Take(request.Length).ToList();

                var responseData = new DTODataTablesResponse<DTOGetApplResponse>
                {
                    draw = request.Draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = paginatedData
                };

                return Json(responseData);
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { error = "An error occurred while loading data: " + ex.Message });
            }
        }

        public async Task<IActionResult> DownloadApplication([FromQuery] List<int> id)
        {
            DTOExportRequest dTOExport = new DTOExportRequest
            {
                Id = id,
            };

            var ret = await _onlineApplication.GetApplicationDetailsForExport(dTOExport);

            // Base path to clean and create new folder
            string basePath = Path.Combine("wwwroot", "PdfDownloaded");

            // Clean old folders/files
            if (Directory.Exists(basePath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(basePath);
                foreach (var dir in dirInfo.GetDirectories())
                {
                    dir.Delete(true);
                }

                foreach (var file in dirInfo.GetFiles())
                {
                    file.Delete();
                }
            }

            // Create new time-based folder
            string newFolderName = CreateFolder(basePath);
            string newFolderPath = Path.Combine(basePath, newFolderName);
            Directory.CreateDirectory(newFolderPath);

            // Create subfolders HBA, CA, PCA inside new folder
            string hbaFolder = Path.Combine(newFolderPath, "HBA");
            string caFolder = Path.Combine(newFolderPath, "CA");
            string pcaFolder = Path.Combine(newFolderPath, "PCA");




            foreach (var data in ret.OnlineApplicationResponse)
            {
                //var folderName = $"{data.ApplicationTypeAbbr}_{data.Number}_{data.ApplicationId}";
                var fileName = $"App{data.ApplicationId}{data.Number}.pdf";
                //var fileName = $"{data.ApplicationTypeAbbr}_{data.Number}_{data.ApplicationId}_Merged.pdf";

                var sourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MergePdf", fileName);
                //var sourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TempUploads", folderName, fileName);

                if (System.IO.File.Exists(sourceFilePath))
                {
                    // Choose target subfolder based on ApplicationTypeAbbr
                    string destinationFolder = data.ApplicationTypeAbbr switch
                    {
                        "HBA" => hbaFolder,
                        "CA" => caFolder,
                        "PCA" => pcaFolder,
                        _ => newFolderPath // fallback if unknown
                    };

                    if (data.ApplicationTypeAbbr == "HBA")
                    {
                        Directory.CreateDirectory(hbaFolder);
                    }

                    if (data.ApplicationTypeAbbr == "CA")
                    {
                        Directory.CreateDirectory(caFolder);
                    }

                    if (data.ApplicationTypeAbbr == "PCA")
                    {
                        Directory.CreateDirectory(pcaFolder);
                    }

                    var destinationFilePath = Path.Combine(destinationFolder, fileName);
                    System.IO.File.Copy(sourceFilePath, destinationFilePath, overwrite: true);
                }
            }


            // Generate Excel file and save to timestamp folder
            bool retexcel = await ExportToExcelInFolder(dTOExport, newFolderPath);
            if (!retexcel)
            {
                return Json(Constants.DataNotExport);
            }
            else
            {
                string zipFileName = $"{newFolderPath}.zip";
                createZip(newFolderPath, zipFileName);
                bool updateStatus = await _userApplication.UpdateStatus(dTOExport);
                if (!updateStatus) 
                { 
                    return Json(Constants.DataNotExport);
                }
                return Json(newFolderName);

            }
          
        }

        // Modified method to save Excel file to specific folder instead of returning File result
        public async Task<bool> ExportToExcelInFolder(DTOExportRequest dTOExport, string folderPath)
        {
            DataTable dataTable = await _onlineApplication.GetApplicationDetailsForExcel(dTOExport);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("ExportedData");
                if (dataTable.Rows.Count > 0)
                {
                    worksheet.Cell(1, 1).InsertTable(dataTable);
                    string excelFilePath = Path.Combine(folderPath, "loanDetails.xlsx");
                    workbook.SaveAs(excelFilePath);
                    return true;
                }
                else
                {
                    return false;
                }
                

              
            }
        }

        public async Task<bool> ClaimExportToExcelInFolder(DTOExportRequest dTOExport, string folderPath)
        {
            DataTable dataTable = await _IClaimonlineApplication1.GetApplicationDetailsForExcel(dTOExport);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("ExportedData");
                if (dataTable.Rows.Count > 0)
                {
                    worksheet.Cell(1, 1).InsertTable(dataTable);
                    string excelFilePath = Path.Combine(folderPath, "loanDetails.xlsx");
                    workbook.SaveAs(excelFilePath);
                    return true;
                }
                else
                {
                    return false;
                }



            }
        }




        // Keep the original method if you need it for direct download
        public async Task<IActionResult> ExportToExcel(DTOExportRequest dTOExport)
        {
            DataTable dataTable = await _onlineApplication.GetApplicationDetailsForExcel(dTOExport);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("ExportedData");
                worksheet.Cell(1, 1).InsertTable(dataTable);

                using (var memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    memoryStream.Position = 0;

                    return File(memoryStream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "loanDetails.xlsx");
                }
            }
        }

        public void createZip(string sourceFolderPath,string destinationFilePath)
        {
            
            if (System.IO.File.Exists(destinationFilePath))
            {
                System.IO.File.Delete(destinationFilePath);
            }

            // Create the zip
            ZipFile.CreateFromDirectory(sourceFolderPath, destinationFilePath, System.IO.Compression.CompressionLevel.Optimal, includeBaseDirectory: false);

        }
        public string CreateFolder(string basePath)
        {
            // Timestamp-based folder name
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string folderPath = Path.Combine(basePath, timestamp);

            // Create the directory
            Directory.CreateDirectory(folderPath);

            return timestamp;
        }

        public async Task<IActionResult> GetApplicationByDate(string date)
        {
            DateTime exportDate = Convert.ToDateTime(date);
            var result = await _userApplication.GetApplicationByDate(exportDate);
            return Json(result);
        }
        
        public async Task<IActionResult> DownloadClaimApplication([FromQuery] List<int> id)
        {
            DTOExportRequest dTOExport = new DTOExportRequest
            {
                Id = id,
            };

            var ret = await _IClaimonlineApplication1.GetApplicationDetailsForExport(dTOExport);

            // Base path to clean and create new folder
            string basePath = Path.Combine("wwwroot", "ClaimPdfDownloaded");

            // Clean old folders/files
            if (Directory.Exists(basePath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(basePath);
                foreach (var dir in dirInfo.GetDirectories())
                {
                    dir.Delete(true);
                }

                foreach (var file in dirInfo.GetFiles())
                {
                    file.Delete();
                }
            }

            // Create new time-based folder
            string newFolderName = CreateFolder(basePath);
            string newFolderPath = Path.Combine(basePath, newFolderName);
            Directory.CreateDirectory(newFolderPath);

            // Create subfolders HBA, CA, PCA inside new folder
            string EDFolder = Path.Combine(newFolderPath, "ED");
            string MWFolder = Path.Combine(newFolderPath, "MW");
            string PRFolder = Path.Combine(newFolderPath, "PR");
            string SPFolder = Path.Combine(newFolderPath, "SP");

            string applicationTypeName = string.Empty;


            foreach (var data in ret.OnlineApplicationResponse)
            {
                if (data.ApplicationType == 1)
                {
                    applicationTypeName = "ED";
                }
                else if (data.ApplicationType == 2)
                {
                    applicationTypeName = "MW";
                }
                else if (data.ApplicationType == 3)
                {
                    applicationTypeName = "PR";
                }
                else if (data.ApplicationType == 4)
                    applicationTypeName = "SP";


                 //var folderName = $"{data.ApplicationTypeAbbr}_{data.Number}_{data.ApplicationId}";
                var fileName = $"App{data.ApplicationId}{data.Number}.pdf";
                //var fileName = $"{data.ApplicationTypeAbbr}_{data.Number}_{data.ApplicationId}_Merged.pdf";

                var sourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ClaimMergePdf", fileName);

                //var folderName = $"{applicationTypeName}_{data.Number}_{data.ApplicationId}";
                //var fileName = $"{applicationTypeName}_{data.Number}_{data.ApplicationId}_Merged.pdf";

                //var sourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ClaimTempUploads", folderName, fileName);

                if (System.IO.File.Exists(sourceFilePath))
                {
                    // Choose target subfolder based on ApplicationTypeAbbr
                    string destinationFolder = applicationTypeName switch
                    {
                        "ED" => EDFolder,
                        "MW" => MWFolder,
                        "PR" => PRFolder,
                        "SP" => SPFolder,
                        _ => newFolderPath // fallback if unknown
                    };

                    if (applicationTypeName == "ED")
                        Directory.CreateDirectory(EDFolder);
                    if (applicationTypeName == "MW")
                        Directory.CreateDirectory(MWFolder);
                    if (applicationTypeName == "PR")
                        Directory.CreateDirectory(PRFolder);
                    if (applicationTypeName == "SP")
                        Directory.CreateDirectory(SPFolder);

                    var destinationFilePath = Path.Combine(destinationFolder, fileName);
                    System.IO.File.Copy(sourceFilePath, destinationFilePath, overwrite: true);
                }
            }

            // Generate Excel file and save to timestamp folder
            bool retexcel = await ClaimExportToExcelInFolder(dTOExport, newFolderPath);
            if (!retexcel)
            {
                return Json(Constants.DataNotExport);
            }
            else
            {
                string zipFileName = $"{newFolderPath}.zip";
                createZip(newFolderPath, zipFileName);
                bool updateStatus = await _userApplication.UpdateClaimStatus(dTOExport);
                if (!updateStatus)
                {
                    return Json(Constants.DataNotExport);
                }
                return Json(newFolderName);

            }
        

        }

        public async Task<IActionResult> GetClaimApplicationByDate(string date)
        {
            DateTime exportDate = Convert.ToDateTime(date);
            var result = await _userApplication.GetClaimApplicationByDate(exportDate);
            return Json(result);
        }

    }
}
