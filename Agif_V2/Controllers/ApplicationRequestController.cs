using Agif_V2.Helpers;
using ClosedXML.Excel;
using DataAccessLayer.Interfaces;
using DataTransferObject.Helpers;
using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Data;
using System.Drawing;

using System.IO.Compression;
using System.Net.Http.Headers;
namespace Agif_V2.Controllers
{
    public class ApplicationRequestController : Controller
    {
        private readonly IUsersApplications _userApplication;
        private readonly IOnlineApplication _onlineApplication;
        private readonly IApplication _application;
        private readonly IUserProfile _userProfile;
        private readonly IClaimOnlineApplication _IClaimonlineApplication1;

        public ApplicationRequestController(IUsersApplications usersApplications, IOnlineApplication _onlineApplication, IApplication _application, IUserProfile _userProfile, IClaimOnlineApplication claimOnlineApplication)
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
        [Authorize(Roles = "CO")]
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
                dTOTempSession.MappingId = dTOUserProfileResponse.MappingId;
            }

            return View(dTOTempSession);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(SessionUserDTO sessionUserDTO)
        {
            var result = await _userApplication.UpdateUserDetails(sessionUserDTO);
            if (result)
            {
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

                await _IClaimonlineApplication1.UpdateApplicationStatus(applId, 102);


                TrnStatusCounter trnStatusCounter = new TrnStatusCounter
                {
                    StatusId = 102,
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
            await _IClaimonlineApplication1.UpdateApplicationStatus(applId, 103);
            TrnStatusCounter trnStatusCounter = new TrnStatusCounter
            {
                StatusId = 103,
                ApplicationId = applId,
                ActionOn = DateTime.Now,
            };
            await _onlineApplication.InsertStatusCounter(trnStatusCounter);

            return Json(new { success = true, message = "Application rejected." });
        }
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "MaturityAdmin")]
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
                        //(x.Name != null && x.Name.ToLower().Contains(searchValue)) ||
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
                        //(x.Name != null && x.Name.ToLower().Contains(searchValue)) ||
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
                var fileName = $"App{data.ApplicationId}{data.Number}.pdf";

                var sourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MergePdf", fileName);

                if (System.IO.File.Exists(sourceFilePath))
                {
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

        public void createZip(string sourceFolderPath, string destinationFilePath)
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

            // Create subfolders ED, MW, PR and SP inside new folder
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

                var fileName = $"App{data.ApplicationId}{data.Number}.pdf";

                var sourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ClaimMergePdf", fileName);


                if (System.IO.File.Exists(sourceFilePath))
                {
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

        public async Task<IActionResult> UploadExcelFile(IFormFile file)
        {
            // Initialize the list properly
            DTOApplStatusBulkUploadlst lst = new DTOApplStatusBulkUploadlst
            {
                DTOApplStatusBulkUploadOK = new List<DTOApplStatusBulkUpload>(),
                DTOApplStatusBulkUploadNotOk = new List<DTOApplStatusBulkUpload>()
            };

            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "Please select a valid Excel file." });
            }

            try
            {
                string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ExcelTempUploads");
                if (Directory.Exists(tempPath))
                {
                    var existingFiles = Directory.GetFiles(tempPath);
                    foreach (var existingFile in existingFiles)
                    {
                        System.IO.File.Delete(existingFile); // Delete existing files
                    }
                }
                else
                {
                    Directory.CreateDirectory(tempPath);
                }

                string filename = $"ExcelImport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                string extension = Path.GetExtension(file.FileName);
                string filePath = Path.Combine(tempPath, filename);

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(stream);
                }

                var requiredHeaders = new List<string> { "Army No", "Appln_ID", "ApplicationType", "Status_Code", "AGIF Remarks" };
                var foundHeaders = new List<string>();

                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return Json(new { success = false, message = "No worksheets found in the Excel file." });
                    }

                    var firstRow = worksheet.Row(1);
                    if (firstRow == null)
                    {
                        return Json(new { success = false, message = "Excel file appears to be empty." });
                    }

                    foreach (var cell in firstRow.CellsUsed())
                    {
                        var header = cell.GetString()?.Trim();
                        if (!string.IsNullOrEmpty(header))
                        {
                            foundHeaders.Add(header);
                        }
                    }
                }

                // Check missing headers
                var missingHeaders = requiredHeaders
                    .Where(required => !foundHeaders.Contains(required, StringComparer.OrdinalIgnoreCase))
                    .ToList();

                if (missingHeaders.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "The following required columns are missing: " + string.Join(", ", missingHeaders)
                    });
                }

                DataTable dt = ExcelToDatatable(filePath);
                if (dt == null || dt.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "No data found in Excel file." });
                }

                var AllStatusCode = await _userApplication.GetAllStatusCode();
                if (AllStatusCode == null)
                {
                    return Json(new { success = false, message = "Unable to retrieve status codes." });
                }

                var applicationIds = dt.AsEnumerable()
                    .Select(row => row.Field<string>("Appln_ID"))
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Distinct()
                    .ToList();

                var NotApp_id = await _userApplication.GetNotApplId(applicationIds);

                foreach (DataRow rw in dt.Rows)
                {
                    string applicationId = rw.Field<string>("Appln_ID");
                    string statusCode = rw.Field<string>("Status_Code");

                    if (string.IsNullOrEmpty(applicationId) || string.IsNullOrEmpty(statusCode))
                    {
                        continue;
                    }

                    // Validate that they can be converted to integers
                    if (!int.TryParse(applicationId, out int applIdInt) || !int.TryParse(statusCode, out int statusCodeInt))
                    {
                        string reason = "";
                        if (!int.TryParse(applicationId, out _))
                        {
                            reason += "Invalid Application ID format. ";
                        }
                        if (!int.TryParse(statusCode, out _))
                        {
                            reason += "Invalid Status Code format.";
                        }

                        lst.DTOApplStatusBulkUploadNotOk.Add(new DTOApplStatusBulkUpload
                        {
                            ApplId = 0, // Or skip setting
                            Status_Code = 0, // Or skip setting
                            ArmyNo = rw.Field<string>("Army No")?.Trim() ?? string.Empty,
                            Remarks = rw.Field<string>("AGIF Remarks")?.Trim() ?? string.Empty,
                            Reason = reason.Trim(),
                            Name = rw.Field<string>("Name")?.Trim() ?? string.Empty
                        });
                        continue;
                    }


                    // Check if the application ID is valid
                    if (NotApp_id.Contains(applicationId))
                    {
                        // Handle invalid application ID
                        lst.DTOApplStatusBulkUploadNotOk.Add(new DTOApplStatusBulkUpload
                        {
                            ApplId = applIdInt,
                            Status_Code = statusCodeInt,
                            ArmyNo = rw.Field<string>("Army No")?.Trim() ?? string.Empty,
                            Remarks = rw.Field<string>("AGIF Remarks")?.Trim() ?? string.Empty,
                            Reason = "Invalid Application ID",
                            Name = rw.Field<string>("Name")?.Trim() ?? string.Empty
                        });
                    }
                    else
                    {
                        // Check if the status code is valid
                        if (AllStatusCode.Contains(statusCode))
                        {
                            // Add to the valid list
                            lst.DTOApplStatusBulkUploadOK.Add(new DTOApplStatusBulkUpload
                            {
                                ApplId = applIdInt,
                                ArmyNo = rw.Field<string>("Army No")?.Trim() ?? string.Empty,
                                Status_Code = statusCodeInt,
                                Remarks = rw.Field<string>("AGIF Remarks")?.Trim() ?? string.Empty,
                                Name = rw.Field<string>("Name")?.Trim() ?? string.Empty
                            });
                        }
                        else
                        {
                            // Handle invalid status code
                            lst.DTOApplStatusBulkUploadNotOk.Add(new DTOApplStatusBulkUpload
                            {
                                ApplId = applIdInt,
                                Status_Code = statusCodeInt,
                                ArmyNo = rw.Field<string>("Army No")?.Trim() ?? string.Empty,
                                Remarks = rw.Field<string>("AGIF Remarks")?.Trim() ?? string.Empty,
                                Reason = "Invalid Status Code",
                                Name = rw.Field<string>("Name")?.Trim() ?? string.Empty
                            });
                        }
                    }
                }
                TempData["BulkUploadOK"] = JsonConvert.SerializeObject(lst.DTOApplStatusBulkUploadOK);
                return Json(new
                {
                    success = true,
                    message = "File uploaded successfully.",
                    Data = lst
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading file: " + ex.Message });
            }
        }
        public async Task<IActionResult> ProcessBulkApplications()
        {
            var tempDataOK = TempData["BulkUploadOK"]?.ToString();
            if (string.IsNullOrEmpty(tempDataOK))
            {
                return Json(new { success = false, message = "No valid applications to process." });
            }

            var lst = JsonConvert.DeserializeObject<List<DTOApplStatusBulkUpload>>(tempDataOK);
            if (lst == null || lst.Count == 0)
            {
                return Json(new { success = false, message = "No valid applications to process." });
            }

            DataTable applicationUpdatesTable = _userApplication.CreateApplicationUpdatesDataTable(lst);

            // Call the bulk update method
            var result = await _userApplication.ProcessBulkApplicationUpdates(applicationUpdatesTable);

            if (result.Item1) // Assuming result is a tuple (bool, string)
            {
                return Json(new { success = true, message = "Bulk applications processed successfully." });
            }
            else
            {
                return Json(new { success = false, message = result.Item2 });
            }

            // Unreachable code removed
        }

        public DataTable ExcelToDatatable(string filePath)
        {
            DataTable dt = new DataTable();
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheets.First();
                var firstRowUsed = worksheet.FirstRowUsed();
                var rowCount = worksheet.LastRowUsed().RowNumber();
                var columnCount = worksheet.LastColumnUsed().ColumnNumber();

                for (int col = 1; col <= columnCount; col++)
                {
                    dt.Columns.Add(worksheet.Cell(1, col).GetString().Trim(), typeof(string));
                }
                //Read data rows
                for (int row = 2; row <= rowCount; row++)
                {
                    var dataRow = dt.NewRow();
                    for (int col = 1; col <= columnCount; col++)
                    {
                        dataRow[col - 1] = worksheet.Cell(row, col).GetString().Trim();
                    }
                    dt.Rows.Add(dataRow);
                }
            }
            return dt;
        }

        [HttpPost]
        public IActionResult ExportValidatedExcel([FromBody] List<DTOApplStatusBulkUpload> data)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Validated");

            worksheet.Cell(1, 1).Value = "Application ID";
            worksheet.Cell(1, 2).Value = "Army No";
            worksheet.Cell(1, 3).Value = "Name";
            worksheet.Cell(1, 4).Value = "Status";

            for (int i = 0; i < data.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = data[i].ApplId;
                worksheet.Cell(i + 2, 2).Value = data[i].ArmyNo;
                worksheet.Cell(i + 2, 3).Value = data[i].Name;
                worksheet.Cell(i + 2, 4).Value = data[i].Status_Code;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ValidatedRecords.xlsx");
        }

        [HttpPost]
        public IActionResult ExportRejectedExcel([FromBody] List<DTOApplStatusBulkUpload> data)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Rejected");

            worksheet.Cell(1, 1).Value = "Application ID";
            worksheet.Cell(1, 2).Value = "Army No";
            worksheet.Cell(1, 3).Value = "Name";
            worksheet.Cell(1, 4).Value = "Status";
            worksheet.Cell(1, 5).Value = "Reason";

            for (int i = 0; i < data.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = data[i].ApplId;
                worksheet.Cell(i + 2, 2).Value = data[i].ArmyNo;
                worksheet.Cell(i + 2, 3).Value = data[i].Name;
                worksheet.Cell(i + 2, 4).Value = data[i].Status_Code;
                worksheet.Cell(i + 2, 5).Value = data[i].Reason;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RejectedRecords.xlsx");
        }


    }
}
