using Agif_V2.Helpers;
using DataAccessLayer.Interfaces;
using DataTransferObject.Helpers;
using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;

namespace Agif_V2.Controllers
{
    public class ApplicationRequestController : Controller
    {
        private readonly IUsersApplications _userApplication;
        private readonly IOnlineApplication _onlineApplication;
        private readonly IApplication _application;
        private readonly OnlineApplicationController _onlineApplicationController;
        
        public ApplicationRequestController(IUsersApplications usersApplications, IOnlineApplication _onlineApplication, IApplication _application,OnlineApplicationController _onlineApplicationController)
        {
            _userApplication = usersApplications;
            this._onlineApplication = _onlineApplication;
            this._application = _application;
            this._onlineApplicationController = _onlineApplicationController;
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
            SessionUserDTO? coSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "CO");
            ViewBag.ArmyNo = coSession.ArmyNo;
            //var app = await _userApplication.GetUsersApplication(dTOTempSession.MappingId, 1);
            return View(dTOTempSession);
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

        public async Task<DTODigitalSignDataResponse> SignDocument(int applicationId)
        {
            DTOCommonOnlineApplicationResponse data = await _onlineApplication.GetApplicationDetails(applicationId);
            DTODigitalSignDataResponse digitalSignDTO = new DTODigitalSignDataResponse();


            if (data.OnlineApplicationResponse != null)
            {
                digitalSignDTO.ApplicationId = data.OnlineApplicationResponse.ApplicationId;
                digitalSignDTO.ArmyNo = data.OnlineApplicationResponse.Number;
                digitalSignDTO.ApplicantName = data.OnlineApplicationResponse.ApplicantName;
                //digitalSignDTO.IsRejectced = con.xmlSignModels.FirstOrDefault(x => x.ApplId == carPcModel.Application_Id)?.IsRejectced == true;
                digitalSignDTO.PCDA_PAO = data.OnlineApplicationResponse.pcda_pao;
                digitalSignDTO.Date_Of_Birth = data.OnlineApplicationResponse.DateOfBirth.ToString();
                digitalSignDTO.Retirement_Date = data.OnlineApplicationResponse.DateOfRetirement.ToString();
                digitalSignDTO.Mobile_No = data.OnlineApplicationResponse.MobileNo;
                digitalSignDTO.ApplType = data.OnlineApplicationResponse.ApplicationType;
                digitalSignDTO.DateOfCommision = data.OnlineApplicationResponse.DateOfCommission.ToString();
                digitalSignDTO.AccountNo = data.OnlineApplicationResponse.SalaryAcctNo;
                digitalSignDTO.RankName = data.OnlineApplicationResponse.DdlRank;
                digitalSignDTO.UnitName = data.OnlineApplicationResponse.PresentUnit;
                digitalSignDTO.PAN_No = data.OnlineApplicationResponse.PanCardNo;
                return digitalSignDTO;

            }
            else
            {
                return null;
            }

        }

        public async Task SaveXML(int applId, string xmlResString, string remarks)
        {
            try
            {
                DTOCommonOnlineApplicationResponse data = await _onlineApplication.GetApplicationDetails(applId);

                var dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "CO");
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

                await _onlineApplication.UpdateApplicationStatus(applId, 2); // This is the likely failing line
                await _application.Add(digitalSignRecords);
                await _onlineApplicationController.MergePdf(applId, false, true);
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
            await _onlineApplicationController.MergePdf(applId, true, false);
            return Json(new { success = true, message = "Application rejected." });

        }

    }
}
