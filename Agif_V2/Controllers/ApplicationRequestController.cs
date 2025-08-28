using Agif_V2.Helpers;
using ClosedXML.Excel;
using DataAccessLayer.Interfaces;
using DataTransferObject.Helpers;
using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using iText.Kernel.Pdf;
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
        private readonly IClaimApplication _claimApplication;
        private readonly Watermark _watermark;
        public ApplicationRequestController(IUsersApplications usersApplications, IOnlineApplication _onlineApplication, IApplication _application, IUserProfile _userProfile, IClaimOnlineApplication claimOnlineApplication, IClaimApplication claimApplication, Watermark watermark)
        {
            _userApplication = usersApplications;
            this._onlineApplication = _onlineApplication;
            this._application = _application;
            this._userProfile = _userProfile;
            this._IClaimonlineApplication1 = claimOnlineApplication;
            _claimApplication = claimApplication;
            _watermark = watermark;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "CO")]
        public IActionResult UserApplicationList(int status)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }
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

            DTOUserProfileResponse? dTOUserProfileResponse = null;

            if (dTOTempSession != null && !string.IsNullOrEmpty(sessionUser?.UserName))
            {
                dTOUserProfileResponse = await _userProfile.GetUserAllDetails(sessionUser.UserName);
            }

            if (dTOUserProfileResponse != null && dTOTempSession != null)
            {
                dTOTempSession.ArmyNo = dTOUserProfileResponse.ArmyNo ?? string.Empty;
                dTOTempSession.MobileNo = dTOUserProfileResponse.MobileNo ?? string.Empty;
                dTOTempSession.ProfileName = dTOUserProfileResponse.ProfileName ?? string.Empty;
                dTOTempSession.UserName = dTOUserProfileResponse.DomainId ?? string.Empty;
                dTOTempSession.EmailId = dTOUserProfileResponse.EmailId ?? string.Empty;
                dTOTempSession.RankId = dTOUserProfileResponse.RankId;
                dTOTempSession.RegtId = dTOUserProfileResponse.RegtId;
                dTOTempSession.ApptId = dTOUserProfileResponse.ApptId;
                dTOTempSession.UnitId = dTOUserProfileResponse.UnitId;
                dTOTempSession.name = dTOUserProfileResponse.username ?? string.Empty;
                dTOTempSession.DteFmn = dTOUserProfileResponse.IsFmn;
                dTOTempSession.MappingId = dTOUserProfileResponse.MappingId;
            }

            return View(dTOTempSession);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(SessionUserDTO sessionUserDTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }
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

        //public async Task<IActionResult> GetUsersApplicationList(DTODataTableRequest request, int status)
        //{

        //    SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
        //    if (dTOTempSession == null || dTOTempSession.MappingId <= 0)
        //    {
        //        return Unauthorized("Session expired or invalid user session.");
        //    }

        //    var queryableData = await _userApplication.GetUsersApplication(dTOTempSession.MappingId, status);

        //    var totalRecords = queryableData.Count;

        //    var query = queryableData.AsQueryable();

        //    if (!string.IsNullOrEmpty(request.searchValue))
        //    {
        //        string searchValue = request.searchValue.ToLower();
        //        query = query.Where(x =>
        //            x.ArmyNo.Contains(searchValue, StringComparison.CurrentCultureIgnoreCase) ||
        //            x.DateOfBirth.Contains(searchValue, StringComparison.CurrentCultureIgnoreCase) ||
        //            x.AppliedDate.Contains(searchValue, StringComparison.CurrentCultureIgnoreCase)
        //        );
        //    }



        //    var filteredRecords = query.Count();

        //    if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
        //    {
        //        bool ascending = request.sortDirection.ToLower() == "asc";

        //        query = request.sortColumn.ToLower() switch
        //        {
        //            "name" => ascending ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name),
        //            "armyno" => ascending ? query.OrderBy(x => x.ArmyNo) : query.OrderByDescending(x => x.ArmyNo),
        //            "dateofbirth" => ascending ? query.OrderBy(x => x.DateOfBirth) : query.OrderByDescending(x => x.DateOfBirth),
        //            "applieddate" => ascending ? query.OrderBy(x => x.AppliedDate) : query.OrderByDescending(x => x.AppliedDate),
        //            _ => query // Default: no sorting if column not recognized
        //        };
        //    }

        //    // Paginate the result
        //    var paginatedData = query.Skip(request.Start).Take(request.Length).ToList();

        //    var responseData = new DTODataTablesResponse<DTOGetApplResponse>
        //    {
        //        draw = request.Draw,
        //        recordsTotal = totalRecords,
        //        recordsFiltered = filteredRecords,
        //        data = paginatedData
        //    };

        //    return Json(responseData);
        //}
        public async Task<IActionResult> GetUsersApplicationList(DTODataTableRequest request, int status)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }
            SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
            if (dTOTempSession == null || dTOTempSession.MappingId <= 0)
            {
                return Unauthorized("Session expired or invalid user session.");
            }

            var queryableData = await _userApplication.GetUsersApplication(dTOTempSession.MappingId, status);
            var query = queryableData.AsQueryable();

            // Apply search filter if provided
            query = ApplySearchFilter(query, request.searchValue);

            // Apply sorting if needed
            query = ApplySorting(query, request.sortColumn, request.sortDirection);

            var totalRecords = queryableData.Count;
            var filteredRecords = query.Count();

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

        // Updated method to handle potential null reference for 'searchValue'
        private IQueryable<DTOGetApplResponse> ApplySearchFilter(IQueryable<DTOGetApplResponse> query, string? searchValue)
        {
            if (string.IsNullOrEmpty(searchValue)) return query;

            string lowerSearchValue = searchValue.ToLower();
            return query.Where(x =>
                (x.ArmyNo ?? string.Empty).Contains(lowerSearchValue, StringComparison.CurrentCultureIgnoreCase) ||
                (x.DateOfBirth ?? string.Empty).Contains(lowerSearchValue, StringComparison.CurrentCultureIgnoreCase) ||
                (x.AppliedDate ?? string.Empty).Contains(lowerSearchValue, StringComparison.CurrentCultureIgnoreCase) 
            );
        }

        // Separate method for sorting logic
        private IQueryable<DTOGetApplResponse> ApplySorting(IQueryable<DTOGetApplResponse> query, string sortColumn, string sortDirection)
        {
            if (string.IsNullOrEmpty(sortColumn) || string.IsNullOrEmpty(sortDirection)) return query;

            bool ascending = sortDirection.ToLower() == "asc";
            return sortColumn.ToLower() switch
            {
                "name" => ascending ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name),
                "armyno" => ascending ? query.OrderBy(x => x.ArmyNo) : query.OrderByDescending(x => x.ArmyNo),
                "dateofbirth" => ascending ? query.OrderBy(x => x.DateOfBirth) : query.OrderByDescending(x => x.DateOfBirth),
                "applieddate" => ascending ? query.OrderBy(x => x.AppliedDate) : query.OrderByDescending(x => x.AppliedDate),
                "digitalsigndate"=> ascending ? query.OrderBy(x => x.DigitalSignDate) : query.OrderByDescending(x => x.DigitalSignDate), 
                _ => query // Default: no sorting if column not recognized
            };
        }


        public async Task<IActionResult> GetMaturityUsersApplicationList(DTODataTableRequest request, int status)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data.");
            }
            SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
            if (dTOTempSession == null || dTOTempSession.MappingId <= 0)
            {
                return Unauthorized("Session expired or invalid user session.");
            }

            var queryableData = await _userApplication.GetMaturityUsersApplication(dTOTempSession.MappingId, status);

            var query = queryableData.AsQueryable();

            // Apply search filter if provided
            query = ApplySearchFilter(query, request.searchValue);

            // Apply sorting if needed
            query = ApplySorting(query, request.sortColumn, request.sortDirection);

            var totalRecords = queryableData.Count;
            var filteredRecords = query.Count();

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

        public string DataDigitalXmlSign(int applicationId)
           {
            if (!ModelState.IsValid)
            {
                return "<Root><Error>Invalid application ID</Error></Root>";
            }
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
        public string ClaimDataDigitalXmlSign(int applicationId)
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



                var digitalSignRecords = new ClaimDigitalSignRecords
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


                TrnClaimStatusCounter trnStatusCounter = new TrnClaimStatusCounter
                {
                    StatusId = 102,
                    ApplicationId = applId,
                    ActionOn = DateTime.Now,
                };
                await _IClaimonlineApplication1.InsertStatusCounter(trnStatusCounter);

                await _claimApplication.Add(digitalSignRecords);

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
            var digitalSignRecords = new ClaimDigitalSignRecords
            {
                ApplId = applId,
                SignOn = DateTime.Now,
                Remarks = rem,
                IsSign = false,
                IsRejectced = true,
            };
            await _claimApplication.Add(digitalSignRecords);

            await _IClaimonlineApplication1.UpdateApplicationStatus(applId, 103);

            TrnClaimStatusCounter trnStatusCounter = new TrnClaimStatusCounter
            {
                StatusId = 103,
                ApplicationId = applId,
                ActionOn = DateTime.Now,
            };

            await _IClaimonlineApplication1.InsertStatusCounter(trnStatusCounter);

            return Json(new { success = true, message = "Application rejected." });
        }
        [Authorize(Roles = "Admin")]
        public IActionResult UsersApplicationListAdmin(string status)
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
        public  IActionResult ClaimApplicationListAdmin(string status)
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


        //public async Task<IActionResult> GetUsersApplicationListToAdmin(DTODataTableRequest request, int status)
        //{
        //    try
        //    {
        //        var queryableData = await _userApplication.GetUsersApplicationForAdmin(status);
        //        var totalRecords = queryableData.Count();
        //        var query = queryableData.AsQueryable();

        //        // Apply search filter - fixed to match actual DTO properties
        //        if (!string.IsNullOrEmpty(request.searchValue))
        //        {
        //            string searchValue = request.searchValue.ToLower();
        //            query = query.Where(x =>
        //                //(x.Name != null && x.Name.ToLower().Contains(searchValue)) ||
        //                (x.ArmyNo != null && x.ArmyNo.Contains(searchValue, StringComparison.CurrentCultureIgnoreCase)) ||
        //                (x.RegtCorps != null && x.RegtCorps.Contains(searchValue, StringComparison.CurrentCultureIgnoreCase)) ||
        //                (x.PresentUnit != null && x.PresentUnit.Contains(searchValue, StringComparison.CurrentCultureIgnoreCase)) ||
        //                (x.AppliedDate != null && x.AppliedDate.Contains(searchValue, StringComparison.CurrentCultureIgnoreCase))
        //            );
        //        }

        //        var filteredRecords = query.Count();

        //        // Apply sorting - fixed to match JavaScript column names
        //        if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
        //        {
        //            bool ascending = string.Equals(request.sortDirection, "asc", StringComparison.OrdinalIgnoreCase); // Case-insensitive comparison for sort direction
        //            query = string.Equals(request.sortColumn, "name", StringComparison.OrdinalIgnoreCase) ?
        //                (ascending ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name)) :
        //            string.Equals(request.sortColumn, "armyno", StringComparison.OrdinalIgnoreCase) ?
        //                (ascending ? query.OrderBy(x => x.ArmyNo) : query.OrderByDescending(x => x.ArmyNo)) :
        //            string.Equals(request.sortColumn, "regtname", StringComparison.OrdinalIgnoreCase) ?
        //                (ascending ? query.OrderBy(x => x.RegtCorps) : query.OrderByDescending(x => x.RegtCorps)) :
        //            string.Equals(request.sortColumn, "presentunit", StringComparison.OrdinalIgnoreCase) ?
        //                (ascending ? query.OrderBy(x => x.PresentUnit) : query.OrderByDescending(x => x.PresentUnit)) :
        //            string.Equals(request.sortColumn, "applieddate", StringComparison.OrdinalIgnoreCase) ?
        //                (ascending ? query.OrderBy(x => x.AppliedDate) : query.OrderByDescending(x => x.AppliedDate)) :
        //                query.OrderByDescending(x => x.UpdatedOn); // Default sorting
        //        }
        //        else
        //        {
        //            // Default sorting by UpdatedOn descending
        //            query = query.OrderByDescending(x => x.UpdatedOn);
        //        }

        //        // Paginate the result
        //        var paginatedData = query.Skip(request.Start).Take(request.Length).ToList();

        //        var responseData = new DTODataTablesResponse<DTOGetApplResponse>
        //        {
        //            draw = request.Draw,
        //            recordsTotal = totalRecords,
        //            recordsFiltered = filteredRecords,
        //            data = paginatedData
        //        };

        //        return Json(responseData);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //        return Json(new { error = "An error occurred while loading data: " + ex.Message });
        //    }
        //}
        public async Task<IActionResult> GetUsersApplicationListToAdmin(DTODataTableRequest request, int status)
        {
            try
            {
                var queryableData = await _userApplication.GetUsersApplicationForAdmin(status);
                var totalRecords = queryableData.Count;
                var query = queryableData.AsQueryable();

                // Apply search filter
                query = AdminApplySearchFilter(query, request.searchValue);

                var filteredRecords = query.Count();

                // Apply sorting
                query = AdminApplySorting(query, request.sortColumn, request.sortDirection);

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

        // Method to apply search filter
        private IQueryable<DTOGetApplResponse> AdminApplySearchFilter(IQueryable<DTOGetApplResponse> query, string? searchValue)
        {
            if (string.IsNullOrEmpty(searchValue)) return query;

            string lowerSearchValue = searchValue.ToLower();
            return query.Where(x =>
                (x.ArmyNo ?? string.Empty).Contains(lowerSearchValue, StringComparison.CurrentCultureIgnoreCase) ||
                (x.RegtCorps ?? string.Empty).Contains(lowerSearchValue, StringComparison.CurrentCultureIgnoreCase) ||
                (x.PresentUnit ?? string.Empty).Contains(lowerSearchValue, StringComparison.CurrentCultureIgnoreCase) ||
                (x.AppliedDate ?? string.Empty).Contains(lowerSearchValue, StringComparison.CurrentCultureIgnoreCase)
            );
        }

        // Method to apply sorting
        private IQueryable<DTOGetApplResponse> AdminApplySorting(IQueryable<DTOGetApplResponse> query, string sortColumn, string sortDirection)
        {
            if (string.IsNullOrEmpty(sortColumn) || string.IsNullOrEmpty(sortDirection)) return query;

            bool ascending = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase); // Case-insensitive comparison for sort direction

            switch (sortColumn.ToLower())
            {
                case "name":
                    return ascending ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
                case "armyno":
                    return ascending ? query.OrderBy(x => x.ArmyNo) : query.OrderByDescending(x => x.ArmyNo);
                case "regtname":
                    return ascending ? query.OrderBy(x => x.RegtCorps) : query.OrderByDescending(x => x.RegtCorps);
                case "presentunit":
                    return ascending ? query.OrderBy(x => x.PresentUnit) : query.OrderByDescending(x => x.PresentUnit);
                case "applieddate":
                    return ascending ? query.OrderBy(x => x.AppliedDate) : query.OrderByDescending(x => x.AppliedDate);
                default:
                    return query.OrderByDescending(x => x.UpdatedOn); // Default sorting by UpdatedOn descending
            }
        }


        public async Task<IActionResult> GetClaimUsersApplicationListToAdmin(DTODataTableRequest request, int status)
        {
            try
            {
                var queryableData = await _userApplication.GetClaimUsersApplicationForAdmin(status);
                var totalRecords = queryableData.Count;
                var query = queryableData.AsQueryable();

                // Apply search filter - fixed to match actual DTO properties
                query = AdminApplySearchFilter(query, request.searchValue);

                var filteredRecords = query.Count();

                // Apply sorting
                query = AdminApplySorting(query, request.sortColumn, request.sortDirection);

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

        //public async Task<IActionResult> DownloadApplication([FromQuery] List<int> id)
        //{
        //    DTOExportRequest dTOExport = new DTOExportRequest
        //    {
        //        Id = id,
        //    };

        //    var ret = await _onlineApplication.GetApplicationDetailsForExport(dTOExport);

        //    // Base path to clean and create new folder
        //    string basePath = Path.Combine("wwwroot", "PdfDownloaded");

        //    // Clean old folders/files
        //    if (Directory.Exists(basePath))
        //    {
        //        DirectoryInfo dirInfo = new DirectoryInfo(basePath);
        //        foreach (var dir in dirInfo.GetDirectories())
        //        {
        //            dir.Delete(true);
        //        }

        //        foreach (var file in dirInfo.GetFiles())
        //        {
        //            file.Delete();
        //        }
        //    }

        //    // Create new time-based folder
        //    string newFolderName = CreateFolder(basePath);
        //    string newFolderPath = Path.Combine(basePath, newFolderName);
        //    Directory.CreateDirectory(newFolderPath);

        //    // Create subfolders HBA, CA, PCA inside new folder
        //    string hbaFolder = Path.Combine(newFolderPath, "HBA");
        //    string caFolder = Path.Combine(newFolderPath, "CA");
        //    string pcaFolder = Path.Combine(newFolderPath, "PCA");




        //    foreach (var data in ret.OnlineApplicationResponse)
        //    {
        //        var fileName = $"App{data.ApplicationId}{data.Number}.pdf";

        //        var sourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MergePdf", fileName);

        //        if (System.IO.File.Exists(sourceFilePath))
        //        {
        //            string destinationFolder = data.ApplicationTypeAbbr switch
        //            {
        //                "HBA" => hbaFolder,
        //                "CA" => caFolder,
        //                "PCA" => pcaFolder,
        //                _ => newFolderPath // fallback if unknown
        //            };

        //            if (data.ApplicationTypeAbbr == "HBA")
        //            {
        //                Directory.CreateDirectory(hbaFolder);
        //            }

        //            if (data.ApplicationTypeAbbr == "CA")
        //            {
        //                Directory.CreateDirectory(caFolder);
        //            }

        //            if (data.ApplicationTypeAbbr == "PCA")
        //            {
        //                Directory.CreateDirectory(pcaFolder);
        //            }

        //            var destinationFilePath = Path.Combine(destinationFolder, fileName);
        //            System.IO.File.Copy(sourceFilePath, destinationFilePath, overwrite: true);
        //        }
        //    }
        //    bool retexcel = await ExportToExcelInFolder(dTOExport, newFolderPath);
        //    if (!retexcel)
        //    {
        //        return Json(Constants.DataNotExport);
        //    }
        //    else
        //    {
        //        string zipFileName = $"{newFolderPath}.zip";
        //        createZip(newFolderPath, zipFileName);
        //        bool updateStatus = await _userApplication.UpdateStatus(dTOExport);
        //        if (!updateStatus)
        //        {
        //            return Json(Constants.DataNotExport);
        //        }
        //        return Json(newFolderName);

        //    }

        //}
        public async Task<IActionResult> DownloadApplication([FromQuery] List<int> id)
        {
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = "IpAddress";
            }
            DTOExportRequest dTOExport = new DTOExportRequest { Id = id };
            var ret = await _onlineApplication.GetApplicationDetailsForExport(dTOExport);

            var armyNo = ret.OnlineApplicationResponse.FirstOrDefault()?.Number ?? "UnknownArmyNo";
            int applicationId = ret.OnlineApplicationResponse.FirstOrDefault()?.ApplicationId ?? 0;
            string fileName = "App"+ applicationId.ToString()+ armyNo+".pdf";
            var mergedPdfPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MergePdf", fileName);

            string basePath = Path.Combine("wwwroot", "PdfDownloaded");
            CleanExistingFiles(basePath);
            string newFolderName = CreateFolder(basePath);
            string newFolderPath = Path.Combine(basePath, newFolderName);
            Directory.CreateDirectory(newFolderPath);

            // Optional: keep the download/watermarked copy in a subfolder
            string downloadFolder = Path.Combine(newFolderPath, "Downloads");
            Directory.CreateDirectory(downloadFolder);

            // Make a copy of the merged PDF in the export folder
            string watermarkedCopyPath = Path.Combine(downloadFolder, fileName);
            System.IO.File.Copy(mergedPdfPath, watermarkedCopyPath, overwrite: true);

            // Watermark the copy (not the original)
            _watermark.AddAnnotationAfterDigitalSign(ipAddress, watermarkedCopyPath);


            string hbaFolder = Path.Combine(newFolderPath, "HBA");
            string caFolder = Path.Combine(newFolderPath, "CA");
            string pcaFolder = Path.Combine(newFolderPath, "PCA");


            // Process and copy files
            bool isFilesCopied = CopyFilesToSubfolders(ret, newFolderPath,hbaFolder,caFolder,pcaFolder, downloadFolder);

            //_watermark.AddAnnotationAfterDigitalSign(ipAddress, newFolderPath);
            // If file copy failed, return
            if (!isFilesCopied)
            {
                return Json(Constants.DataNotExport);
            }

            // Export to Excel in the new folder
            bool retexcel = await ExportToExcelInFolder(dTOExport, newFolderPath);
            if (!retexcel)
            {
                return Json(Constants.DataNotExport);
            }

            // Create a zip file
            string zipFileName = CreateZipFile(newFolderPath);

            // Update application status
            bool updateStatus = await _userApplication.UpdateStatus(dTOExport);
            if (!updateStatus)
            {
                return Json(Constants.DataNotExport);
            }

            return Json(newFolderName);
        }

        // Helper methods

        private void CleanExistingFiles(string basePath)
        {
            if (Directory.Exists(basePath))
            {
                var dirInfo = new DirectoryInfo(basePath);
                foreach (var dir in dirInfo.GetDirectories())
                {
                    dir.Delete(true);  // Clean existing directories
                }

                foreach (var file in dirInfo.GetFiles())
                {
                    file.Delete();  // Clean existing files
                }
            }
        }

     

        private bool CopyFilesToSubfolders(dynamic ret, string newFolderPath,string hbaFolder,string caFolder,string pcaFolder,string watermarkFolderPath)
        {
            bool isFilesCopied = true;

            foreach (var data in ret.OnlineApplicationResponse)
            {
                var fileName = $"App{data.ApplicationId}{data.Number}.pdf";
                //var sourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MergePdf", fileName);
                var sourceFilePath = Path.Combine(watermarkFolderPath, fileName);

                if (System.IO.File.Exists(sourceFilePath))
                {
                    string destinationFolder = data.ApplicationTypeAbbr switch
                    {
                        "HBA" => Path.Combine(newFolderPath, "HBA"),
                        "CA" => Path.Combine(newFolderPath, "CA"),
                        "PCA" => Path.Combine(newFolderPath, "PCA"),
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
                    try
                    {
                        var destinationFilePath = Path.Combine(destinationFolder, fileName);
                        System.IO.File.Copy(sourceFilePath, destinationFilePath, overwrite: true);
                    }
                    catch (Exception)
                    {
                        isFilesCopied = false;
                    }
                }
            }
            return isFilesCopied;
        }

        private string CreateZipFile(string newFolderPath)
        {
            string zipFileName = $"{newFolderPath}.zip";
            createZip(newFolderPath, zipFileName);
            return zipFileName;
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
            if (dataTable.Columns.Contains("EmailDomain"))
            {
                dataTable.Columns.Remove("EmailDomain");
            }
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("ExportedData");
                if (dataTable.Rows.Count > 0)
                {
                    worksheet.Cell(1, 1).InsertTable(dataTable);
                    string excelFilePath = Path.Combine(folderPath, "MAWD_Details.xlsx");
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

        //public async Task<IActionResult> DownloadClaimApplication([FromQuery] List<int> id)
        //{
        //    DTOExportRequest dTOExport = new DTOExportRequest
        //    {
        //        Id = id,
        //    };

        //    var ret = await _IClaimonlineApplication1.GetApplicationDetailsForExport(dTOExport);

        //    // Base path to clean and create new folder
        //    string basePath = Path.Combine("wwwroot", "ClaimPdfDownloaded");

        //    // Clean old folders/files
        //    if (Directory.Exists(basePath))
        //    {
        //        DirectoryInfo dirInfo = new DirectoryInfo(basePath);
        //        foreach (var dir in dirInfo.GetDirectories())
        //        {
        //            dir.Delete(true);
        //        }

        //        foreach (var file in dirInfo.GetFiles())
        //        {
        //            file.Delete();
        //        }
        //    }

        //    // Create new time-based folder
        //    string newFolderName = CreateFolder(basePath);
        //    string newFolderPath = Path.Combine(basePath, newFolderName);
        //    Directory.CreateDirectory(newFolderPath);

        //    // Create subfolders ED, MW, PR and SP inside new folder
        //    string EDFolder = Path.Combine(newFolderPath, "ED");
        //    string MWFolder = Path.Combine(newFolderPath, "MW");
        //    string PRFolder = Path.Combine(newFolderPath, "PR");
        //    string SPFolder = Path.Combine(newFolderPath, "SP");

        //    string applicationTypeName = string.Empty;


        //    foreach (var data in ret.OnlineApplicationResponse ?? Enumerable.Empty<ClaimCommonDataOnlineResponse>())
        //    {
        //        if (data.ApplicationType == 1)
        //        {
        //            applicationTypeName = "ED";
        //        }
        //        else if (data.ApplicationType == 2)
        //        {
        //            applicationTypeName = "MW";
        //        }
        //        else if (data.ApplicationType == 3)
        //        {
        //            applicationTypeName = "PR";
        //        }
        //        else if (data.ApplicationType == 4)
        //            applicationTypeName = "SP";

        //        var fileName = $"App{data.ApplicationId}{data.Number}.pdf";

        //        var sourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ClaimMergePdf", fileName);


        //        if (System.IO.File.Exists(sourceFilePath))
        //        {
        //            string destinationFolder = applicationTypeName switch
        //            {
        //                "ED" => EDFolder,
        //                "MW" => MWFolder,
        //                "PR" => PRFolder,
        //                "SP" => SPFolder,
        //                _ => newFolderPath // fallback if unknown
        //            };

        //            if (applicationTypeName == "ED")
        //                Directory.CreateDirectory(EDFolder);
        //            if (applicationTypeName == "MW")
        //                Directory.CreateDirectory(MWFolder);
        //            if (applicationTypeName == "PR")
        //                Directory.CreateDirectory(PRFolder);
        //            if (applicationTypeName == "SP")
        //                Directory.CreateDirectory(SPFolder);

        //            var destinationFilePath = Path.Combine(destinationFolder, fileName);
        //            System.IO.File.Copy(sourceFilePath, destinationFilePath, overwrite: true);
        //        }
        //    }

        //    bool retexcel = await ClaimExportToExcelInFolder(dTOExport, newFolderPath);
        //    if (!retexcel)
        //    {
        //        return Json(Constants.DataNotExport);
        //    }
        //    else
        //    {
        //        string zipFileName = $"{newFolderPath}.zip";
        //        createZip(newFolderPath, zipFileName);
        //        bool updateStatus = await _userApplication.UpdateClaimStatus(dTOExport);
        //        if (!updateStatus)
        //        {
        //            return Json(Constants.DataNotExport);
        //        }
        //        return Json(newFolderName);

        //    }

        //}

        public async Task<IActionResult> DownloadClaimApplication([FromQuery] List<int> id)
        {
            DTOExportRequest dTOExport = new DTOExportRequest { Id = id };
            var ret = await _IClaimonlineApplication1.GetApplicationDetailsForExport(dTOExport);

            // Define base path
            string basePath = Path.Combine("wwwroot", "ClaimPdfDownloaded");

            // Clean old folders/files
            CleanExistingFiles(basePath);

            // Create new folder and subfolders   
            string newFolderName = CreateFolder(basePath);

            string newFolderPath = Path.Combine(basePath, newFolderName);
            Directory.CreateDirectory(newFolderPath);

            // Create subfolders ED, MW, PR and SP inside new folder
            string EDFolder = Path.Combine(newFolderPath, "ED");
            string MWFolder = Path.Combine(newFolderPath, "MW");
            string PRFolder = Path.Combine(newFolderPath, "PR");
            string SPFolder = Path.Combine(newFolderPath, "SP");

            // Process and copy files to subfolders
            bool isFilesCopied = CopyClaimFilesToSubfolders(ret, newFolderPath, EDFolder, MWFolder, PRFolder, SPFolder);

            if (!isFilesCopied)
            {
                return Json(Constants.DataNotExport);
            }

            // Export to Excel
            bool retexcel = await ClaimExportToExcelInFolder(dTOExport, newFolderPath);
            if (!retexcel)
            {
                return Json(Constants.DataNotExport);
            }

            // Create a zip file
            string zipFileName = CreateZipFile(newFolderPath);

            // Update claim application status
            bool updateStatus = await _userApplication.UpdateClaimStatus(dTOExport);
            if (!updateStatus)
            {
                return Json(Constants.DataNotExport);
            }

            return Json(newFolderName);
        }

        // Helper Methods

        private bool CopyClaimFilesToSubfolders(dynamic ret, string newFolderPath,string EDFolder,string MWFolder,string PRFolder, string SPFolder)
        {
            bool isFilesCopied = true;
            string applicationTypeName = string.Empty;

            foreach (var data in ret.OnlineApplicationResponse ?? Enumerable.Empty<ClaimCommonDataOnlineResponse>())
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

                    try
                   {
                       var destinationFilePath = Path.Combine(destinationFolder, fileName);
                       System.IO.File.Copy(sourceFilePath, destinationFilePath, overwrite: true);
                   }

                   catch (Exception)
                   {
                       isFilesCopied = false;
                   }
               }
           }
             return isFilesCopied;
       
        }



        public async Task<IActionResult> GetClaimApplicationByDate(string date)
        {
            DateTime exportDate = Convert.ToDateTime(date);
            var result = await _userApplication.GetClaimApplicationByDate(exportDate);
            return Json(result);
        }

        //public async Task<IActionResult> UploadExcelFile(IFormFile file)
        //{
        //    // Initialize the list properly
        //    DTOApplStatusBulkUploadlst lst = new DTOApplStatusBulkUploadlst
        //    {
        //        DTOApplStatusBulkUploadOK = new List<DTOApplStatusBulkUpload>(),
        //        DTOApplStatusBulkUploadNotOk = new List<DTOApplStatusBulkUpload>()
        //    };

        //    if (file == null || file.Length == 0)
        //    {
        //        return Json(new { success = false, message = "Please select a valid Excel file." });
        //    }

        //    try
        //    {
        //        string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ExcelTempUploads");
        //        if (Directory.Exists(tempPath))
        //        {
        //            var existingFiles = Directory.GetFiles(tempPath);
        //            foreach (var existingFile in existingFiles)
        //            {
        //                System.IO.File.Delete(existingFile); // Delete existing files
        //            }
        //        }
        //        else
        //        {
        //            Directory.CreateDirectory(tempPath);
        //        }

        //        string filename = $"ExcelImport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        //        string extension = Path.GetExtension(file.FileName);
        //        string filePath = Path.Combine(tempPath, filename);

        //        using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        var requiredHeaders = new List<string> { "Army No", "Appln_ID", "ApplicationType", "Status_Code", "AGIF Remarks" };
        //        var foundHeaders = new List<string>();

        //        using (var workbook = new XLWorkbook(filePath))
        //        {
        //            var worksheet = workbook.Worksheets.FirstOrDefault();
        //            if (worksheet == null)
        //            {
        //                return Json(new { success = false, message = "No worksheets found in the Excel file." });
        //            }

        //            var firstRow = worksheet.Row(1);
        //            if (firstRow == null)
        //            {
        //                return Json(new { success = false, message = "Excel file appears to be empty." });
        //            }

        //            foreach (var cell in firstRow.CellsUsed())
        //            {
        //                var header = cell.GetString()?.Trim();
        //                if (!string.IsNullOrEmpty(header))
        //                {
        //                    foundHeaders.Add(header);
        //                }
        //            }
        //        }

        //        // Check missing headers
        //        var missingHeaders = requiredHeaders
        //            .Where(required => !foundHeaders.Contains(required, StringComparer.OrdinalIgnoreCase))
        //            .ToList();

        //        if (missingHeaders.Any())
        //        {
        //            return Json(new
        //            {
        //                success = false,
        //                message = "The following required columns are missing: " + string.Join(", ", missingHeaders)
        //            });
        //        }

        //        DataTable dt = ExcelToDatatable(filePath);
        //        if (dt == null || dt.Rows.Count == 0)
        //        {
        //            return Json(new { success = false, message = "No data found in Excel file." });
        //        }

        //        var AllStatusCode = await _userApplication.GetAllStatusCode();
        //        if (AllStatusCode == null)
        //        {
        //            return Json(new { success = false, message = "Unable to retrieve status codes." });
        //        }

        //        var applicationIds = dt.AsEnumerable()
        //            .Select(row => row.Field<string>("Appln_ID"))
        //            .Where(id => !string.IsNullOrEmpty(id))
        //            .Distinct()
        //            .ToList();

        //        var NotApp_id = await _userApplication.GetNotApplId(applicationIds);

        //        foreach (DataRow rw in dt.Rows)
        //        {
        //            string? applicationId = rw.Field<string>("Appln_ID");
        //            string? statusCode = rw.Field<string>("Status_Code");

        //            if (string.IsNullOrEmpty(applicationId) || string.IsNullOrEmpty(statusCode))
        //            {
        //                continue;
        //            }

        //            // Validate that they can be converted to integers
        //            if (!int.TryParse(applicationId, out int applIdInt) || !int.TryParse(statusCode, out int statusCodeInt))
        //            {
        //                string reason = "";
        //                if (!int.TryParse(applicationId, out _))
        //                {
        //                    reason += "Invalid Application ID format. ";
        //                }
        //                if (!int.TryParse(statusCode, out _))
        //                {
        //                    reason += "Invalid Status Code format.";
        //                }

        //                lst.DTOApplStatusBulkUploadNotOk.Add(new DTOApplStatusBulkUpload
        //                {
        //                    ApplId = 0, // Or skip setting
        //                    Status_Code = 0, // Or skip setting
        //                    ArmyNo = rw.Field<string>("Army No")?.Trim() ?? string.Empty,
        //                    Remarks = rw.Field<string>("AGIF Remarks")?.Trim() ?? string.Empty,
        //                    Reason = reason.Trim(),
        //                    Name = rw.Field<string>("Name")?.Trim() ?? string.Empty
        //                });
        //                continue;
        //            }


        //            // Check if the application ID is valid
        //            if (NotApp_id.Contains(applicationId))
        //            {
        //                // Handle invalid application ID
        //                lst.DTOApplStatusBulkUploadNotOk.Add(new DTOApplStatusBulkUpload
        //                {
        //                    ApplId = applIdInt,
        //                    Status_Code = statusCodeInt,
        //                    ArmyNo = rw.Field<string>("Army No")?.Trim() ?? string.Empty,
        //                    Remarks = rw.Field<string>("AGIF Remarks")?.Trim() ?? string.Empty,
        //                    Reason = "Invalid Application ID",
        //                    Name = rw.Field<string>("Name")?.Trim() ?? string.Empty
        //                });
        //            }
        //            else
        //            {
        //                // Check if the status code is valid
        //                if (AllStatusCode.Contains(statusCode))
        //                {
        //                    // Add to the valid list
        //                    lst.DTOApplStatusBulkUploadOK.Add(new DTOApplStatusBulkUpload
        //                    {
        //                        ApplId = applIdInt,
        //                        ArmyNo = rw.Field<string>("Army No")?.Trim() ?? string.Empty,
        //                        Status_Code = statusCodeInt,
        //                        Remarks = rw.Field<string>("AGIF Remarks")?.Trim() ?? string.Empty,
        //                        Name = rw.Field<string>("Name")?.Trim() ?? string.Empty
        //                    });
        //                }
        //                else
        //                {
        //                    // Handle invalid status code
        //                    lst.DTOApplStatusBulkUploadNotOk.Add(new DTOApplStatusBulkUpload
        //                    {
        //                        ApplId = applIdInt,
        //                        Status_Code = statusCodeInt,
        //                        ArmyNo = rw.Field<string>("Army No")?.Trim() ?? string.Empty,
        //                        Remarks = rw.Field<string>("AGIF Remarks")?.Trim() ?? string.Empty,
        //                        Reason = "Invalid Status Code",
        //                        Name = rw.Field<string>("Name")?.Trim() ?? string.Empty
        //                    });
        //                }
        //            }
        //        }
        //        TempData["BulkUploadOK"] = JsonConvert.SerializeObject(lst.DTOApplStatusBulkUploadOK);
        //        return Json(new
        //        {
        //            success = true,
        //            message = "File uploaded successfully.",
        //            Data = lst
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Error uploading file: " + ex.Message });
        //    }
        //}

        public async Task<IActionResult> UploadExcelFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "Please select a valid Excel file." });

            DTOApplStatusBulkUploadlst lst = InitializeUploadList();
            string filePath;

            try
            {
                filePath = await SaveFileTempAsync(file);
                var missingHeaders = CheckExcelHeaders(filePath, new List<string> { "Army No", "Appln_ID", "ApplicationType", "Status_Code", "AGIF Remarks" });

                if (missingHeaders.Any())
                    return Json(new { success = false, message = "The following required columns are missing: " + string.Join(", ", missingHeaders) });

                DataTable dt = ExcelToDatatable(filePath);

                if (dt == null || dt.Rows.Count == 0)
                    return Json(new { success = false, message = "No data found in Excel file." });

                var allStatusCode = await _userApplication.GetAllStatusCode();
                if (allStatusCode == null)
                    return Json(new { success = false, message = "Unable to retrieve status codes." });

                var applicationIds = dt.AsEnumerable()
                    .Select(row => row.Field<string>("Appln_ID"))
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Distinct()
                    .ToList();

                var notAppIds = await _userApplication.GetNotApplId(applicationIds);

                ProcessRows(dt, lst, allStatusCode, notAppIds);

                TempData["BulkUploadOK"] = JsonConvert.SerializeObject(lst.DTOApplStatusBulkUploadOK);

                return Json(new { success = true, message = "File uploaded successfully.", Data = lst });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading file: " + ex.Message });
            }
        }

        // ---------------- Helper Methods ----------------

        private DTOApplStatusBulkUploadlst InitializeUploadList()
        {
            return new DTOApplStatusBulkUploadlst
            {
                DTOApplStatusBulkUploadOK = new List<DTOApplStatusBulkUpload>(),
                DTOApplStatusBulkUploadNotOk = new List<DTOApplStatusBulkUpload>()
            };
        }

        private async Task<string> SaveFileTempAsync(IFormFile file)
        {
            string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ExcelTempUploads");

            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);
            else
            {
                foreach (var existingFile in Directory.GetFiles(tempPath))
                    System.IO.File.Delete(existingFile);
            }

            string filename = $"ExcelImport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string filePath = Path.Combine(tempPath, filename);

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        private List<string> CheckExcelHeaders(string filePath, List<string> requiredHeaders)
        {
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet == null) throw new Exception("No worksheets found in the Excel file.");

            var foundHeaders = worksheet.Row(1).CellsUsed().Select(c => c.GetString()?.Trim()).Where(h => !string.IsNullOrEmpty(h)).ToList();

            return requiredHeaders
                .Where(required => !foundHeaders.Contains(required, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        private void ProcessRows(DataTable dt, DTOApplStatusBulkUploadlst lst, List<string> allStatusCode, List<string> notAppIds)
        {
            foreach (DataRow row in dt.Rows)
            {
                string? applIdStr = row.Field<string>("Appln_ID");
                string? statusCodeStr = row.Field<string>("Status_Code");

                if (string.IsNullOrEmpty(applIdStr) || string.IsNullOrEmpty(statusCodeStr))
                    continue;

                bool validApplId = int.TryParse(applIdStr, out int applId);
                bool validStatusCode = int.TryParse(statusCodeStr, out int statusCode);

                if (!validApplId || !validStatusCode)
                {
                    lst.DTOApplStatusBulkUploadNotOk.Add(CreateNotOkRow(row, validApplId, validStatusCode));
                    continue;
                }

                if (notAppIds.Contains(applIdStr))
                {
                    lst.DTOApplStatusBulkUploadNotOk.Add(CreateNotOkRow(row, applId, statusCode, "Invalid Application ID"));
                }
                else if (allStatusCode.Contains(statusCodeStr))
                {
                    lst.DTOApplStatusBulkUploadOK.Add(CreateOkRow(row, applId, statusCode));
                }
                else
                {
                    lst.DTOApplStatusBulkUploadNotOk.Add(CreateNotOkRow(row, applId, statusCode, "Invalid Status Code"));
                }
            }
        }

        private DTOApplStatusBulkUpload CreateOkRow(DataRow row, int applId, int statusCode)
        {
            return new DTOApplStatusBulkUpload
            {
                ApplId = applId,
                ArmyNo = row.Field<string>("Army No")?.Trim() ?? string.Empty,
                Status_Code = statusCode,
                Remarks = row.Field<string>("AGIF Remarks")?.Trim() ?? string.Empty,
                Name = row.Field<string>("Name")?.Trim() ?? string.Empty
            };
        }

        private DTOApplStatusBulkUpload CreateNotOkRow(DataRow row, bool validApplId, bool validStatusCode)
        {
            string reason = "";
            if (!validApplId) reason += "Invalid Application ID format. ";
            if (!validStatusCode) reason += "Invalid Status Code format.";

            return new DTOApplStatusBulkUpload
            {
                ApplId = 0,
                Status_Code = 0,
                ArmyNo = row.Field<string>("Army No")?.Trim() ?? string.Empty,
                Remarks = row.Field<string>("AGIF Remarks")?.Trim() ?? string.Empty,
                Reason = reason.Trim(),
                Name = row.Field<string>("Name")?.Trim() ?? string.Empty
            };
        }

        private DTOApplStatusBulkUpload CreateNotOkRow(DataRow row, int applId, int statusCode, string reason)
        {
            return new DTOApplStatusBulkUpload
            {
                ApplId = applId,
                Status_Code = statusCode,
                ArmyNo = row.Field<string>("Army No")?.Trim() ?? string.Empty,
                Remarks = row.Field<string>("AGIF Remarks")?.Trim() ?? string.Empty,
                Reason = reason,
                Name = row.Field<string>("Name")?.Trim() ?? string.Empty
            };
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

        //public async Task<IActionResult> ClaimUploadExcelFile(IFormFile file)
        //{
        //    // Initialize the list properly
        //    DTOApplStatusBulkUploadlst lst = new DTOApplStatusBulkUploadlst
        //    {
        //        DTOApplStatusBulkUploadOK = new List<DTOApplStatusBulkUpload>(),
        //        DTOApplStatusBulkUploadNotOk = new List<DTOApplStatusBulkUpload>()
        //    };

        //    if (file == null || file.Length == 0)
        //    {
        //        return Json(new { success = false, message = "Please select a valid Excel file." });
        //    }

        //    try
        //    {
        //        string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ClaimExcelTempUploads");
        //        if (Directory.Exists(tempPath))
        //        {
        //            var existingFiles = Directory.GetFiles(tempPath);
        //            foreach (var existingFile in existingFiles)
        //            {
        //                System.IO.File.Delete(existingFile); // Delete existing files
        //            }
        //        }
        //        else
        //        {
        //            Directory.CreateDirectory(tempPath);
        //        }

        //        string filename = $"ExcelImport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        //        string extension = Path.GetExtension(file.FileName);
        //        string filePath = Path.Combine(tempPath, filename);

        //        using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        var requiredHeaders = new List<string> { "Army No", "Appln_ID", "ApplicationType", "Status_Code", "AGIF Remarks" };
        //        var foundHeaders = new List<string>();

        //        using (var workbook = new XLWorkbook(filePath))
        //        {
        //            var worksheet = workbook.Worksheets.FirstOrDefault();
        //            if (worksheet == null)
        //            {
        //                return Json(new { success = false, message = "No worksheets found in the Excel file." });
        //            }

        //            var firstRow = worksheet.Row(1);
        //            if (firstRow == null)
        //            {
        //                return Json(new { success = false, message = "Excel file appears to be empty." });
        //            }

        //            foreach (var cell in firstRow.CellsUsed())
        //            {
        //                var header = cell.GetString()?.Trim();
        //                if (!string.IsNullOrEmpty(header))
        //                {
        //                    foundHeaders.Add(header);
        //                }
        //            }
        //        }

        //        // Check missing headers
        //        var missingHeaders = requiredHeaders
        //            .Where(required => !foundHeaders.Contains(required, StringComparer.OrdinalIgnoreCase))
        //            .ToList();

        //        if (missingHeaders.Any())
        //        {
        //            return Json(new
        //            {
        //                success = false,
        //                message = "The following required columns are missing: " + string.Join(", ", missingHeaders)
        //            });
        //        }

        //        DataTable dt = ExcelToDatatable(filePath);
        //        if (dt == null || dt.Rows.Count == 0)
        //        {
        //            return Json(new { success = false, message = "No data found in Excel file." });
        //        }

        //        var AllStatusCode = await _userApplication.GetAllClaimStatusCode();
        //        if (AllStatusCode == null)
        //        {
        //            return Json(new { success = false, message = "Unable to retrieve status codes." });
        //        }

        //        var applicationIds = dt.AsEnumerable()
        //            .Select(row => row.Field<string>("Appln_ID"))
        //            .Where(id => !string.IsNullOrEmpty(id))
        //            .Distinct()
        //            .ToList();

        //        var NotApp_id = await _userApplication.GetClaimNotApplId(applicationIds);

        //        foreach (DataRow rw in dt.Rows)
        //        {
        //            string? applicationId = rw.Field<string>("Appln_ID");
        //            string? statusCode = rw.Field<string>("Status_Code");

        //            if (string.IsNullOrEmpty(applicationId) || string.IsNullOrEmpty(statusCode))
        //            {
        //                continue;
        //            }

        //            // Validate that they can be converted to integers
        //            if (!int.TryParse(applicationId, out int applIdInt) || !int.TryParse(statusCode, out int statusCodeInt))
        //            {
        //                string reason = "";
        //                if (!int.TryParse(applicationId, out _))
        //                {
        //                    reason += "Invalid Application ID format. ";
        //                }
        //                if (!int.TryParse(statusCode, out _))
        //                {
        //                    reason += "Invalid Status Code format.";
        //                }

        //                lst.DTOApplStatusBulkUploadNotOk.Add(new DTOApplStatusBulkUpload
        //                {
        //                    ApplId = 0, // Or skip setting
        //                    Status_Code = 0, // Or skip setting
        //                    ArmyNo = rw.Field<string>("Army No")?.Trim() ?? string.Empty,
        //                    Remarks = rw.Field<string>("AGIF Remarks")?.Trim() ?? string.Empty,
        //                    Reason = reason.Trim(),
        //                    Name = rw.Field<string>("Name")?.Trim() ?? string.Empty
        //                });
        //                continue;
        //            }


        //            // Check if the application ID is valid
        //            if (NotApp_id.Contains(applicationId))
        //            {
        //                // Handle invalid application ID
        //                lst.DTOApplStatusBulkUploadNotOk.Add(new DTOApplStatusBulkUpload
        //                {
        //                    ApplId = applIdInt,
        //                    Status_Code = statusCodeInt,
        //                    ArmyNo = rw.Field<string>("Army No")?.Trim() ?? string.Empty,
        //                    Remarks = rw.Field<string>("AGIF Remarks")?.Trim() ?? string.Empty,
        //                    Reason = "Invalid Application ID",
        //                    Name = rw.Field<string>("Name")?.Trim() ?? string.Empty
        //                });
        //            }
        //            else
        //            {
        //                // Check if the status code is valid
        //                if (AllStatusCode.Contains(statusCode))
        //                {
        //                    // Add to the valid list
        //                    lst.DTOApplStatusBulkUploadOK.Add(new DTOApplStatusBulkUpload
        //                    {
        //                        ApplId = applIdInt,
        //                        ArmyNo = rw.Field<string>("Army No")?.Trim() ?? string.Empty,
        //                        Status_Code = statusCodeInt,
        //                        Remarks = rw.Field<string>("AGIF Remarks")?.Trim() ?? string.Empty,
        //                        Name = rw.Field<string>("Name")?.Trim() ?? string.Empty
        //                    });
        //                }
        //                else
        //                {
        //                    // Handle invalid status code
        //                    lst.DTOApplStatusBulkUploadNotOk.Add(new DTOApplStatusBulkUpload
        //                    {
        //                        ApplId = applIdInt,
        //                        Status_Code = statusCodeInt,
        //                        ArmyNo = rw.Field<string>("Army No")?.Trim() ?? string.Empty,
        //                        Remarks = rw.Field<string>("AGIF Remarks")?.Trim() ?? string.Empty,
        //                        Reason = "Invalid Status Code",
        //                        Name = rw.Field<string>("Name")?.Trim() ?? string.Empty
        //                    });
        //                }
        //            }
        //        }
        //        TempData["ClaimBulkUploadOK"] = JsonConvert.SerializeObject(lst.DTOApplStatusBulkUploadOK);
        //        return Json(new
        //        {
        //            success = true,
        //            message = "File uploaded successfully.",
        //            Data = lst
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Error uploading file: " + ex.Message });
        //    }
        //}

        public async Task<IActionResult> ClaimUploadExcelFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "Please select a valid Excel file." });

            DTOApplStatusBulkUploadlst lst = InitializeUploadList();
            string filePath;

            try
            {
                filePath = await SaveClaimFileTempAsync(file);
                var missingHeaders = CheckExcelHeaders(filePath, new List<string> { "Army No", "Appln_ID", "ApplicationType", "Status_Code", "AGIF Remarks" });

                if (missingHeaders.Any())
                    return Json(new { success = false, message = "The following required columns are missing: " + string.Join(", ", missingHeaders) });

                DataTable dt = ExcelToDatatable(filePath);

                if (dt == null || dt.Rows.Count == 0)
                    return Json(new { success = false, message = "No data found in Excel file." });

                var allStatusCode = await _userApplication.GetAllClaimStatusCode();
                if (allStatusCode == null)
                    return Json(new { success = false, message = "Unable to retrieve status codes." });

                var applicationIds = dt.AsEnumerable()
                    .Select(row => row.Field<string>("Appln_ID"))
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Distinct()
                    .ToList();

                var notAppIds = await _userApplication.GetClaimNotApplId(applicationIds);

                ProcessRows(dt, lst, allStatusCode, notAppIds);

                TempData["ClaimBulkUploadOK"] = JsonConvert.SerializeObject(lst.DTOApplStatusBulkUploadOK);

                return Json(new { success = true, message = "File uploaded successfully.", Data = lst });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading file: " + ex.Message });
            }
        }


        private async Task<string> SaveClaimFileTempAsync(IFormFile file)
        {
            string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ClaimExcelTempUploads");

            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);
            else
            {
                foreach (var existingFile in Directory.GetFiles(tempPath))
                    System.IO.File.Delete(existingFile);
            }

            string filename = $"ExcelImport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            string filePath = Path.Combine(tempPath, filename);

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        public async Task<IActionResult> ClaimProcessBulkApplications()
        {
            var tempDataOK = TempData["ClaimBulkUploadOK"]?.ToString();
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
            var result = await _userApplication.ClaimProcessBulkApplicationUpdates(applicationUpdatesTable);

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


    }
}
