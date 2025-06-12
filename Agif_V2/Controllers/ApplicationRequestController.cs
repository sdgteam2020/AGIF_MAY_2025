using DataAccessLayer.Interfaces;
using DataTransferObject.Helpers;
using DataTransferObject.Request;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Mvc;

namespace Agif_V2.Controllers
{
    public class ApplicationRequestController : Controller
    {
        private readonly IUsersApplications _userApplication;
        public ApplicationRequestController(IUsersApplications usersApplications)
        {
            _userApplication = usersApplications;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> UserApplicationList(int status)
        {
            ViewBag.Status = status;
            //SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
            //if (dTOTempSession == null || dTOTempSession.ProfileId <= 0)
            //{
            //    return Unauthorized("Session expired or invalid user session.");
            //}
            //var app = await _userApplication.GetUsersApplication(dTOTempSession.MappingId, 1);
            return View();
        }
        public async Task<IActionResult> GetUsersApplicationList(DTODataTableRequest request, int Type)
        {

            SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
            if (dTOTempSession == null || dTOTempSession.ProfileId <= 0)
            {
                return Unauthorized("Session expired or invalid user session.");
            }

            var queryableData = await _userApplication.GetUsersApplication(dTOTempSession.ProfileId, Type);

            var totalRecords = queryableData.Count();

            var query = queryableData.AsQueryable();

            //if (!string.IsNullOrEmpty(request.searchValue))
            //{
            //    string searchValue = request.searchValue.ToLower();
            //    query = query.Where(x =>
            //        x.ProfileName.ToLower().Contains(searchValue) ||
            //        x.EmailId.ToLower().Contains(searchValue) ||
            //        x.MobileNo.ToLower().Contains(searchValue) ||
            //        x.ArmyNo.ToLower().Contains(searchValue) ||
            //        x.UnitName.ToLower().Contains(searchValue) ||
            //        x.AppointmentName.ToLower().Contains(searchValue) ||
            //        x.RegtName.ToLower().Contains(searchValue)
            //    );
            //}

            var filteredRecords = query.Count();

            //if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
            //{
            //    bool ascending = request.sortDirection.ToLower() == "asc";

            //    query = request.sortColumn.ToLower() switch
            //    {
            //        "profilename" => ascending ? query.OrderBy(x => x.ProfileName) : query.OrderByDescending(x => x.ProfileName),
            //        "emailid" => ascending ? query.OrderBy(x => x.EmailId) : query.OrderByDescending(x => x.EmailId),
            //        "mobileno" => ascending ? query.OrderBy(x => x.MobileNo) : query.OrderByDescending(x => x.MobileNo),
            //        "armyno" => ascending ? query.OrderBy(x => x.ArmyNo) : query.OrderByDescending(x => x.ArmyNo),
            //        "unitname" => ascending ? query.OrderBy(x => x.UnitName) : query.OrderByDescending(x => x.UnitName),
            //        "appointmentname" => ascending ? query.OrderBy(x => x.AppointmentName) : query.OrderByDescending(x => x.AppointmentName),
            //        "regtname" => ascending ? query.OrderBy(x => x.RegtName) : query.OrderByDescending(x => x.RegtName),
            //        _ => query // Default: no sorting if column not recognized
            //    };
            //}

            // Paginate the result
            var paginatedData = query.Skip(request.Start).Take(request.Length).ToList();

            var responseData = new DTODataTablesResponse<DTOUserProfileResponse>
            {
                draw = request.Draw,
                recordsTotal = totalRecords,
                recordsFiltered = filteredRecords
            };

            return Json(responseData);

        }
    }
}
