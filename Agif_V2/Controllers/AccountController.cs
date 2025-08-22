using ClosedXML.Excel;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using DataTransferObject.Helpers;
using DataTransferObject.Identitytable;
using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using DocumentFormat.OpenXml.Spreadsheet;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Agif_V2.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserProfile _userProfile;
        private readonly IUserMapping _userMapping;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMasterOnlyTable _masterOnlyTable;

        public AccountController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext db, IUserProfile userProfile, IUserMapping userMapping, IMasterOnlyTable _masterOnlyTable)
        {   
            _signInManager = signInManager;
            _userManager = userManager;
            _userProfile = userProfile;
            _userMapping = userMapping;
            _db = db;
            this._masterOnlyTable = _masterOnlyTable;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetUserAsync(model.UserName);
            if (user == null)
            {
                return HandleInvalidUserName(model);
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                return await HandleLockedOutUser(model, user);
            }

            var result = await SignInUserAsync(model, user);

            if (result.Succeeded)
            {
                return await HandleSuccessfulLogin(user, model);
            }

            return await HandleFailedLogin(result, model, user);
        }


        // Helper method to populate lockout information
        private async Task<LoginViewModel> PopulateLockoutInfo(LoginViewModel model, ApplicationUser user)
        {
            if (user != null)
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                var failedAttempts = await _userManager.GetAccessFailedCountAsync(user);
                var maxAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts;

                model.IsLockedOut = await _userManager.IsLockedOutAsync(user);
                model.LockoutEnd = lockoutEnd?.DateTime;
                model.FailedAttempts = failedAttempts;
                model.MaxAllowedAttempts = maxAttempts;

                if (model.IsLockedOut && lockoutEnd.HasValue)
                {
                    var timeRemaining = lockoutEnd.Value.Subtract(DateTimeOffset.UtcNow);
                    if (timeRemaining.TotalMinutes > 60)
                    {
                        model.LockoutMessage = $"Account locked for {Math.Ceiling(timeRemaining.TotalHours)} hour(s).";
                    }
                    else if (timeRemaining.TotalSeconds > 0)
                    {
                        model.LockoutMessage = $"Account locked for {Math.Ceiling(timeRemaining.TotalMinutes)} minute(s).";
                    }
                    else
                    {
                        model.LockoutMessage = "Account lockout has expired. Please try again.";
                    }
                }
            }

            return model;
        }


        private async Task<ApplicationUser?> GetUserAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }


        private IActionResult HandleInvalidUserName(LoginViewModel model)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            TempData["UserName"] = model.UserName;
            return RedirectToAction("Register", "Account");
        }


        private async Task<IActionResult> HandleLockedOutUser(LoginViewModel model, ApplicationUser user)
        {
            model = await PopulateLockoutInfo(model, user);
            return View(model);
        }

        private async Task<Microsoft.AspNetCore.Identity.SignInResult> SignInUserAsync(LoginViewModel model, ApplicationUser user)
        {
            user.SecurityStamp = Guid.NewGuid().ToString();
            await _userManager.UpdateAsync(user);
            await _signInManager.SignOutAsync(); // Clear any existing session/cookies

            return await _signInManager.PasswordSignInAsync(
                model.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true // Enable lockout on failure
            );
        }


        private async Task<IActionResult> HandleSuccessfulLogin(ApplicationUser user, LoginViewModel model)
        {
            await _userManager.ResetAccessFailedCountAsync(user);

            var roles = await _userManager.GetRolesAsync(user);
            var profile = await _userProfile.GetUserAllDetails(model.UserName);

            string role = roles.Contains("Admin") ? "Admin" : roles.FirstOrDefault() ?? "User";


            SessionUserDTO sessionUserDTO = new SessionUserDTO
            {
                UserName = user.UserName ?? string.Empty,
                UserId = user.Id,
                ProfileId = profile.ProfileId,
                MappingId = profile.MappingId,
                Role = role,
                DomainId = user.DomainId,
                RankName = profile.RankName ?? string.Empty,
                ArmyNo = profile.ArmyNo ?? string.Empty
            };

            Helpers.SessionExtensions.SetObject(HttpContext.Session, "User", sessionUserDTO);

            if (roles.Contains("Admin") || roles.Contains("MaturityAdmin") || roles.Contains("SuperAdmin"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (profile == null)
            {
                return Json(new { success = false, message = "User mapping not found." });
            }

            bool isCOActive = profile.IsCOActive;
            if (!isCOActive)
            {
                return RedirectToAction("COContactUs", "Default");
            }

            HttpContext.Session.SetString("UserGUID", user.Id.ToString());
            return RedirectToAction("Index", "Default");
        }

        private async Task<IActionResult> HandleFailedLogin(Microsoft.AspNetCore.Identity.SignInResult result, LoginViewModel model, ApplicationUser user)
        {
            if (result.IsLockedOut)
            {
                model = await PopulateLockoutInfo(model, user);
                return View(model);
            }

            if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Your account is not allowed to sign in.");
                return View(model);
            }

            var updatedFailedAttempts = await _userManager.GetAccessFailedCountAsync(user);
            var remainingAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts - updatedFailedAttempts;

            if (remainingAttempts > 0)
            {
                ModelState.AddModelError(string.Empty, $"Invalid username or password. {remainingAttempts} attempt(s) remaining before account lockout.");
            }
            else
            {
                model = await PopulateLockoutInfo(model, user);
            }

            return View(model);
        }
        public IActionResult Register()
        {
            DTOuserProfile userProfileDTO = new DTOuserProfile();

            TempData.Keep("UserName");
            return View(userProfileDTO);
        }


        [HttpPost]
        public async Task<IActionResult> Register(DTOuserProfile signUpDto)
        {
            if (ModelState.IsValid)
            {

                var newUser = new ApplicationUser
                {
                    UserName = signUpDto.userName,
                    Email = signUpDto.Email,
                    PhoneNumber = signUpDto.MobileNo,
                    Updatedby = 1,
                    UpdatedOn = DateTime.Now,
                    DomainId=signUpDto.userName
                };

                var Result = await _userManager.CreateAsync(newUser, "Admin123!");
                if (!Result.Succeeded)
                {
                    return Json(Result.Errors);
                }
                await _userManager.AddToRoleAsync(newUser, "CO");

                await _db.SaveChangesAsync();

                UserProfile userProfile = new UserProfile
                {
                    ArmyNo = signUpDto.ArmyNo,
                    userName = signUpDto.userName,
                    Name = signUpDto.Name,
                    Email = signUpDto.Email,
                    MobileNo = signUpDto.MobileNo,
                    rank = signUpDto.rank,
                    regtCorps = signUpDto.regtCorps,
                    ApptId = signUpDto.ApptId,
                    IsActive = false,
                    UpdatedOn = DateTime.Now
                };
                await _userProfile.Add(userProfile);

                UserMapping user = await _userMapping.GetUnitDetails(signUpDto.UnitId);

                bool IsPrimary = user == null;

                UserMapping userMapping = new UserMapping
                {
                    UserId = Convert.ToInt32(await _userManager.GetUserIdAsync(newUser)),
                    IsActive = false,
                    ProfileId = userProfile.ProfileId,
                    UnitId = signUpDto.UnitId,
                    UpdatedOn = DateTime.Now,
                    IsFmn = signUpDto.DteFmn,
                    IsPrimary = IsPrimary
                };
                await _userMapping.Add(userMapping);
                return RedirectToAction("COContactUs", "Default");
            }
            else
            {
                return View(signUpDto);
            }

        }

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult GetAllUsers(bool status)
        {
            if (!ModelState.IsValid)
            {
                // If the model state is not valid, return an error message or handle accordingly
                return BadRequest("Invalid request.");
            }

            ViewBag.UserStatus = status;
            SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
            if (dTOTempSession == null || dTOTempSession.ProfileId <= 0)
            {
                return Unauthorized("Session expired or invalid user session.");
            }
            return View(dTOTempSession);
        }
        //public async Task<IActionResult> GetAllUsersListPaginated(DTODataTableRequest request, string status = "")
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            // If the request is invalid, return an empty response or a proper error message
        //            var invalidResponse = CreateResponse(0, 0, 0, new List<DTOUserProfileResponse>());
        //            return Json(invalidResponse);
        //        }
        //        bool userStatus = GetUserStatus(status);

        //        var queryableData = await _userProfile.GetAllUser(userStatus);

        //        var totalRecords = queryableData.Count;
        //        var query = queryableData.AsQueryable();

        //        query = ApplySearchFilter(query, request.searchValue);
        //        var filteredRecords = query.Count();

        //        query = ApplySorting(query, request.sortColumn, request.sortDirection);

        //        var paginatedData = query.Skip(request.Start).Take(request.Length).ToList();

        //        var responseData = CreateResponse(request.Draw, totalRecords, filteredRecords, paginatedData);

        //        return Json(responseData);
        //    }
        //    catch (Exception)
        //    {
        //        var responseData = CreateResponse(0, 0, 0, new List<DTOUserProfileResponse>());
        //        return Json(responseData);
        //    }

        //}

        public async Task<IActionResult> GetAllUsersListPaginated(DTODataTableRequest request, string status = "")
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // If the request is invalid, return an empty response or a proper error message
                    var invalidResponse = CreateResponse(0, 0, 0, new List<DTOUserProfileResponse>());
                    return Json(invalidResponse);
                }
                bool userStatus = GetUserStatus(status);

                var query = _userProfile.GetAllUser(userStatus);

                var totalRecords =await query.CountAsync();

                query = ApplySearchFilter(query, request.searchValue);
                var filteredRecords =await query.CountAsync();

                query = ApplySorting(query, request.sortColumn, request.sortDirection);

                var paginatedData = await query.Skip(request.Start).Take(request.Length).ToListAsync();

                var responseData = CreateResponse(request.Draw, totalRecords, filteredRecords, paginatedData);

                return Json(responseData);
            }
            catch (Exception)
            {
                var responseData = CreateResponse(0, 0, 0, new List<DTOUserProfileResponse>());
                return Json(responseData);
            }

        }

        private bool GetUserStatus(string status)
        {
            if (string.IsNullOrEmpty(status)) return false;
            return string.Equals(status, "true", StringComparison.OrdinalIgnoreCase);
        }

        private static string EscapeLike(string input)
{
    // Escape %, _ and [ which have special meaning in SQL LIKE
    return input.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");
}

private IQueryable<DTOUserProfileResponse> ApplySearchFilter(
    IQueryable<DTOUserProfileResponse> query, string? searchValue)
{
    if (string.IsNullOrWhiteSpace(searchValue)) return query;

    var s = EscapeLike(searchValue.Trim());
    var pattern = $"%{s}%";

    // If your DB/columns are case-sensitive, force CI collation per field:
    // var ci = "SQL_Latin1_General_CP1_CI_AS";

    return query.Where(x =>
        EF.Functions.Like(x.EmailId         ?? "", pattern) ||
        EF.Functions.Like(x.MobileNo        ?? "", pattern) ||
        EF.Functions.Like(x.ArmyNo          ?? "", pattern) ||
        EF.Functions.Like(x.UnitName        ?? "", pattern) ||
        EF.Functions.Like(x.AppointmentName ?? "", pattern) ||
        EF.Functions.Like(x.RegtName        ?? "", pattern)
        // Example if your columns are CS and you need CI:
        // EF.Functions.Like(EF.Functions.Collate(x.EmailId ?? "", ci), pattern) || ...
    );
}


        //private IQueryable<DTOUserProfileResponse> ApplySearchFilter(IQueryable<DTOUserProfileResponse> query, string? searchValue)
        //{
        //    if (string.IsNullOrEmpty(searchValue)) return query;

        //    string lowerSearchValue = searchValue.ToLower();
        //    return query.Where(x =>
        //        (x.EmailId ?? string.Empty).Contains(lowerSearchValue, StringComparison.CurrentCultureIgnoreCase) ||
        //        (x.MobileNo ?? string.Empty).Contains(lowerSearchValue, StringComparison.CurrentCultureIgnoreCase) ||
        //        (x.ArmyNo ?? string.Empty).Contains(lowerSearchValue, StringComparison.CurrentCultureIgnoreCase) ||
        //        (x.UnitName ?? string.Empty).Contains(lowerSearchValue, StringComparison.CurrentCultureIgnoreCase) ||
        //        (x.AppointmentName ?? string.Empty).Contains(lowerSearchValue, StringComparison.CurrentCultureIgnoreCase) ||
        //        (x.RegtName ?? string.Empty).Contains(lowerSearchValue, StringComparison.CurrentCultureIgnoreCase)
        //    );
        //}

           private IQueryable<DTOUserProfileResponse> ApplySorting(IQueryable<DTOUserProfileResponse> query, string sortColumn, string sortDirection)
           {
               if (string.IsNullOrEmpty(sortColumn) || string.IsNullOrEmpty(sortDirection)) return query;

               bool ascending = sortDirection.ToLower() == "asc";

               // Define sorting logic in a dictionary
               var sortMap = new Dictionary<string, Func<IQueryable<DTOUserProfileResponse>, IOrderedQueryable<DTOUserProfileResponse>>>
          {
           { "profilename", q => ascending ? q.OrderBy(x => x.ProfileName) : q.OrderByDescending(x => x.ProfileName) },
           { "emailid", q => ascending ? q.OrderBy(x => x.EmailId) : q.OrderByDescending(x => x.EmailId) },
           { "mobileno", q => ascending ? q.OrderBy(x => x.MobileNo) : q.OrderByDescending(x => x.MobileNo) },
           { "armyno", q => ascending ? q.OrderBy(x => x.ArmyNo) : q.OrderByDescending(x => x.ArmyNo) },
           { "unitname", q => ascending ? q.OrderBy(x => x.UnitName) : q.OrderByDescending(x => x.UnitName) },
           { "appointmentname", q => ascending ? q.OrderBy(x => x.AppointmentName) : q.OrderByDescending(x => x.AppointmentName) },
           { "regtname", q => ascending ? q.OrderBy(x => x.RegtName) : q.OrderByDescending(x => x.RegtName) },
           { "isactive", q => ascending ? q.OrderBy(x => x.IsActive) : q.OrderByDescending(x => x.IsActive) },
           { "isprimary", q => ascending ? q.OrderBy(x => x.IsPrimary) : q.OrderByDescending(x => x.IsPrimary) },
           { "isfmn", q => ascending ? q.OrderBy(x => x.IsFmn) : q.OrderByDescending(x => x.IsFmn) }
         };

               // Use the dictionary to apply the sorting
            return sortMap.ContainsKey(sortColumn.ToLower()) ? sortMap[sortColumn.ToLower()](query) : query;
        }

        private DTODataTablesResponse<DTOUserProfileResponse> CreateResponse(int draw, int totalRecords, int filteredRecords, List<DTOUserProfileResponse> data)
        {
            return new DTODataTablesResponse<DTOUserProfileResponse>
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = filteredRecords,
                data = data
            };
        }

        [HttpPost]
        public async Task<IActionResult> ExportAllUsersToExcel(string status = "")
        {
            try
            {
                bool userStatus = true;
                if (!string.IsNullOrEmpty(status))
                {
                    userStatus = status.Equals("true", StringComparison.CurrentCultureIgnoreCase);
                }

                // Get all users data
                var queryableData = _userProfile.GetAllUser(userStatus);
                var userList = queryableData.ToList();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Users");

                    // Add headers and data to worksheet
                    worksheet.Cell(1, 1).Value = "S.No.";
                    worksheet.Cell(1, 2).Value = "Profile Name";
                    worksheet.Cell(1, 3).Value = "Email Id";
                    worksheet.Cell(1, 4).Value = "Mobile No";
                    worksheet.Cell(1, 5).Value = "Army No";
                    worksheet.Cell(1, 6).Value = "Unit Name";
                    worksheet.Cell(1, 7).Value = "Appointment Name";
                    worksheet.Cell(1, 8).Value = "Regiment Name";
                    worksheet.Cell(1, 9).Value = "Active Status";

                    int row = 2;
                    foreach (var user in userList)
                    {
                        worksheet.Cell(row, 1).Value = row - 1;
                        worksheet.Cell(row, 2).Value = user.ProfileName;
                        worksheet.Cell(row, 3).Value = user.EmailId;
                        worksheet.Cell(row, 4).Value = user.MobileNo;
                        worksheet.Cell(row, 5).Value = user.ArmyNo;
                        worksheet.Cell(row, 6).Value = user.UnitName;
                        worksheet.Cell(row, 7).Value = user.AppointmentName;
                        worksheet.Cell(row, 8).Value = user.RegtName;
                        worksheet.Cell(row, 9).Value = user.IsActive ? "Active" : "Inactive";
                        row++;
                    }

                    // Save the file to memory stream
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var fileContents = stream.ToArray();
                        return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UsersList.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while exporting the data. " + ex.Message });
            }
        }

        public async Task<ActionResult> GetALLByUnitName(string UnitName)
        {
           
                var ret = await _masterOnlyTable.GetALLByUnitName(UnitName);
                return Json(ret);         
           
        }

        [HttpPost]
        public async Task<JsonResult> UpdateUserStatus(string domainId, bool isActive)
        {
            var sessionUser = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
            
            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            if (string.IsNullOrWhiteSpace(domainId))
            {
                return Json(new { success = false, message = "Domain ID cannot be null or empty." });
            }

            var userProfile = await _userProfile.GetByUserName(domainId);
            if (userProfile == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var userMapping = (await _userMapping.GetByProfileId(userProfile.ProfileId)).FirstOrDefault();
            if (userMapping == null)
            {
                return Json(new { success = false, message = "User mapping not found." });
            }

            bool result = await _userProfile.SaveApprovedLogs(sessionUser.DomainId, ip, isActive,domainId,userProfile.ProfileId);


            userMapping.IsActive = isActive;
            userMapping.UpdatedOn = DateTime.Now;
            await _userMapping.Update(userMapping);



            return Json(new { success = true });
        }

        public async Task<IActionResult> FinalLogout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> UpdateUserPrimary(string domainId, bool isPrimary)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid request." });
            }

            if (string.IsNullOrWhiteSpace(domainId))
            {
                return Json(new { success = false, message = "Domain ID cannot be null or empty." });
            }

            var userProfile = await _userProfile.GetByUserName(domainId);
            if (userProfile == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var userMapping = (await _userMapping.GetByProfileId(userProfile.ProfileId)).FirstOrDefault();
            if (userMapping == null)
            {
                return Json(new { success = false, message = "User mapping not found." });
            }

            var unitId = userMapping.UnitId;

            if (isPrimary)
            {
                var allUserMappings = await _userMapping.GetByUnitId(unitId);
                foreach (var mapping in allUserMappings)
                {
                    if (mapping.MappingId != userMapping.MappingId)
                    {
                        mapping.IsPrimary = false; // Set other mappings to non-primary
                        await _userMapping.Update(mapping);
                    }
                }

                userMapping.IsPrimary = true;
                await _userMapping.Update(userMapping);
            }

            return Json(new { success = true });
        }

        public async Task<IActionResult> CheckIsCoRegister(int unitId)
        {
            if (!ModelState.IsValid)
            {
                return Json(0);
            }
            try
            {
                var ret = await _userMapping.GetActiveUnitId(unitId);
                if (ret == null || !ret.Any())
                {
                    return Json(0); 
                }
                else
                {
                    return Json(1); 
                }
            }
            catch (Exception)
            {
                return Json(0);
            }
            
        }

        [HttpPost]
        public async Task<ActionResult> GetUnitById(int UnitId)
        {
            if (!ModelState.IsValid)
            {
                return Json("Invalid request");
            }

            try
            {
                if (UnitId > 0)
                {
                    var unit = await _db.MUnits.FirstOrDefaultAsync(i => i.UnitId == UnitId);
                    if (unit != null)
                    {
                        return Json(unit.UnitName);
                    }
                    else
                    {
                        return Json("Unit not found");
                    }
                }
                return Json(0);
            }
            catch (Exception)
            {
                return Json(0);
            }
        }
    }
}
