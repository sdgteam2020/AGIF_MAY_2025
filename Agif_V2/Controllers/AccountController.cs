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


            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                TempData["UserName"] = model.UserName;
                return RedirectToAction("Register","Account");
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                model = await PopulateLockoutInfo(model, user);
                return View(model);
            }

            var currentFailedAttempts = await _userManager.GetAccessFailedCountAsync(user);

            var lockoutOptions = _userManager.Options.Lockout;
            var maxAttempts = lockoutOptions.MaxFailedAccessAttempts;
            var lockoutDuration = lockoutOptions.DefaultLockoutTimeSpan;

            user.SecurityStamp = Guid.NewGuid().ToString();
            await _userManager.UpdateAsync(user);

            await _signInManager.SignOutAsync(); // Clear any existing session/cookies

            var result = await _signInManager.PasswordSignInAsync(
                model.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true // Enable lockout on failure
            );

            if (result.Succeeded)
            {
                await _userManager.ResetAccessFailedCountAsync(user);

                var roles = await _userManager.GetRolesAsync(user);
                var profile = await _userProfile.GetUserAllDetails(model.UserName);

                string role = roles.Contains("Admin") ? "Admin" : roles.FirstOrDefault() ?? "User";
                

                SessionUserDTO sessionUserDTO = new SessionUserDTO
                {
                    UserName = user.UserName,
                    UserId = user.Id,
                    ProfileId = profile.ProfileId,
                    MappingId = profile.MappingId,
                    Role = role,
                    DomainId = user.DomainId,
                    RankName = profile.RankName,
                    ArmyNo = profile.ArmyNo
                };

                Helpers.SessionExtensions.SetObject(HttpContext.Session, "User", sessionUserDTO);

                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Home");
                }
                else if(roles.Contains("MaturityAdmin"))
                {
                    return RedirectToAction("Index", "Home");
                }
                else if(roles.Contains("SuperAdmin"))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (profile == null)
                    {
                        return Json(new { success = false, message = "User mapping not found." });
                    }

                    bool isCOActive = profile.IsCOActive;
                    if (!isCOActive)
                    {
                        return RedirectToAction("COContactUs", "Default");
                    }
                }

                HttpContext.Session.SetString("UserGUID", user.Id.ToString());
                return RedirectToAction("Index", "Default");
            }
            else if (result.IsLockedOut)
            {
                // User is now locked out
                model = await PopulateLockoutInfo(model, user);
                return View(model);
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Your account is not allowed to sign in.");
                return View(model);
            }
            else
            {
                // Failed login attempt
                var updatedFailedAttempts = await _userManager.GetAccessFailedCountAsync(user);
                var remainingAttempts = maxAttempts - updatedFailedAttempts;

                if (remainingAttempts > 0)
                {
                    ModelState.AddModelError(string.Empty,
                        $"Invalid username or password. {remainingAttempts} attempt(s) remaining before account lockout.");
                }
                else
                {
                    model = await PopulateLockoutInfo(model, user);
                }

                return View(model);
            }
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


        public async Task<IActionResult> Register()
        {
            var sessionUser = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");


            DTOuserProfile userProfileDTO = new DTOuserProfile();

            TempData.Keep("UserName");
            return View(userProfileDTO);
        }

        [HttpPost]
        
        public async Task<IActionResult> Register(DTOuserProfile signUpDto)
        {
            if(ModelState.IsValid)
            {

                var newUser = new ApplicationUser
                {
                    UserName = signUpDto.userName,
                    Email = signUpDto.Email,
                    PhoneNumber = signUpDto.MobileNo,
                    Updatedby = 1,
                    UpdatedOn = DateTime.Now,
                };

                var Result = await _userManager.CreateAsync(newUser, "Admin123!");
                if (!Result.Succeeded)
                {
                    return Json(Result.Errors);
                }
                var RoleRet = await _userManager.AddToRoleAsync(newUser, "CO");

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

                UserMapping user = new UserMapping();

                user = await _userMapping.GetUnitDetails(signUpDto.UnitId);

                bool IsPrimary  = user == null;

                UserMapping userMapping = new UserMapping
                {
                    UserId = Convert.ToInt32(await _userManager.GetUserIdAsync(newUser)),
                    IsActive = false,
                    ProfileId = userProfile.ProfileId,
                    UnitId = signUpDto.UnitId,
                    UpdatedOn = DateTime.Now,
                    IsFmn= signUpDto.DteFmn,
                    IsPrimary = IsPrimary ? true : false
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
            ViewBag.UserStatus = status;
            SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
            if (dTOTempSession == null || dTOTempSession.ProfileId <= 0)
            {
                return Unauthorized("Session expired or invalid user session.");
            }
            return View(dTOTempSession);
        }
        public async Task<IActionResult> GetAllUsersListPaginated(DTODataTableRequest request, string status = "")
        {
            try
            {
                bool userStatus = false;
                if (!string.IsNullOrEmpty(status))
                {
                    userStatus = status.ToLower() == "true";
                }

                var queryableData = await _userProfile.GetAllUser(userStatus);

                var totalRecords = queryableData.Count();

                var query = queryableData.AsQueryable();

                if (!string.IsNullOrEmpty(request.searchValue))
                {
                    string searchValue = request.searchValue.ToLower();
                    query = query.Where(x =>
                        x.ProfileName.ToLower().Contains(searchValue) ||
                        x.EmailId.ToLower().Contains(searchValue) ||
                        x.MobileNo.ToLower().Contains(searchValue) ||
                        x.ArmyNo.ToLower().Contains(searchValue) ||
                        x.UnitName.ToLower().Contains(searchValue) ||
                        x.AppointmentName.ToLower().Contains(searchValue) ||
                        x.RegtName.ToLower().Contains(searchValue)
                    );
                }

                var filteredRecords = query.Count();

                if (!string.IsNullOrEmpty(request.sortColumn) && !string.IsNullOrEmpty(request.sortDirection))
                {
                    bool ascending = request.sortDirection.ToLower() == "asc";

                    query = request.sortColumn.ToLower() switch
                    {
                        "profilename" => ascending ? query.OrderBy(x => x.ProfileName) : query.OrderByDescending(x => x.ProfileName),
                        "emailid" => ascending ? query.OrderBy(x => x.EmailId) : query.OrderByDescending(x => x.EmailId),
                        "mobileno" => ascending ? query.OrderBy(x => x.MobileNo) : query.OrderByDescending(x => x.MobileNo),
                        "armyno" => ascending ? query.OrderBy(x => x.ArmyNo) : query.OrderByDescending(x => x.ArmyNo),
                        "unitname" => ascending ? query.OrderBy(x => x.UnitName) : query.OrderByDescending(x => x.UnitName),
                        "appointmentname" => ascending ? query.OrderBy(x => x.AppointmentName) : query.OrderByDescending(x => x.AppointmentName),
                        "regtname" => ascending ? query.OrderBy(x => x.RegtName) : query.OrderByDescending(x => x.RegtName),
                        "isactive" => ascending ? query.OrderBy(x => x.IsActive) : query.OrderByDescending(x => x.IsActive),
                        "isprimary" => ascending ? query.OrderBy(x => x.IsPrimary) : query.OrderByDescending(x => x.IsPrimary),
                        "isfmn" => ascending ? query.OrderBy(x => x.IsFmn) : query.OrderByDescending(x => x.IsFmn),
                        _ => query 
                    };
                }

                // Paginate the result
                var paginatedData = query.Skip(request.Start).Take(request.Length).ToList();

                var responseData = new DTODataTablesResponse<DTOUserProfileResponse>
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
                var responseData = new DTODataTablesResponse<DTOUserProfileResponse>
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<DTOUserProfileResponse>()
                };
                return Json(responseData);
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
            if(string.IsNullOrEmpty(domainId))
            {
                return Json(new { success = false, message = "Domain ID cannot be null or empty." });
            }
            var userProfile = _userProfile.GetByUserName(domainId).Result;
            if(userProfile == null)
            {
                return Json(new { success = false, message = "User not found." });
            }
            int profileId = userProfile.ProfileId;

            var userMapping = _userMapping.GetByProfileId(userProfile.ProfileId).Result.FirstOrDefault();
            if (userMapping == null)
            {
                return Json(new { success = false, message = "User mapping not found." });
            }
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
            if(string.IsNullOrEmpty(domainId))
            {
                return Json(new { success = false, message = "Domain ID cannot be null or empty." });
            }
            var userProfile = _userProfile.GetByUserName(domainId).Result;
            if(userProfile == null)
            {
                return Json(new { success = false, message = "User not found." });
            }
            int profileId = userProfile.ProfileId;

            var userMapping = _userMapping.GetByProfileId(userProfile.ProfileId).Result.FirstOrDefault();
            if (userMapping == null)
            {
                return Json(new { success = false, message = "User mapping not found." });
            }
            var unitId = userMapping.UnitId;
            if (isPrimary)
            {
                var allUserMappings = _userMapping.GetByUnitId(unitId).Result;
                foreach (var mapping in allUserMappings)
                {
                    if (mapping.MappingId != userMapping.MappingId)
                    {
                        mapping.IsPrimary = false; // Set other mappings to non-primary
                        await _userMapping.Update(mapping);
                    }
                }
                userMapping.IsPrimary = isPrimary;
                await _userMapping.Update(userMapping);
            }
            //userMapping.IsPrimary = isPrimary;
            //await _userMapping.Update(userMapping);
            return Json(new { success = true });
        }
        public async Task<IActionResult> CheckIsCoRegister(int unitId)
        {
            try
            {
                var ret = await _userMapping.GetActiveUnitId(unitId);
                if (ret == null || !ret.Any())
                {
                    return Json(0); // No user mapping found for the given unitId
                }
                else
                {
                    return Json(1); // User mapping exists for the given unitId
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
            try
            {
                if (UnitId > 0)
                {

                    var ret = _db.MUnits.Where(i => i.UnitId == UnitId).FirstOrDefault().UnitName;
                    return Json(ret);

                }
                return Json(0);
            }
            catch (Exception ex)
            {
                return Json(0);
            }
        }
    }
}
