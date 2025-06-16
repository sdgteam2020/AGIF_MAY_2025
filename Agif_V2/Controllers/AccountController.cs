using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using DataTransferObject.Helpers;
using DataTransferObject.Identitytable;
using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Agif_V2.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserProfile _userProfile;
        private readonly IUserMapping _userMapping;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public AccountController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext db, IUserProfile userProfile, IUserMapping userMapping)
        {   
            _signInManager = signInManager;
            _userManager = userManager;
            _userProfile = userProfile;
            _userMapping = userMapping;
            _db = db;
        }
        public IActionResult Index()
        {
           
            return View();
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
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);
            if(result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                int userId = Convert.ToInt32(await _userManager.GetUserIdAsync(user));
                var mappingId =  _userMapping.GetByUserId(userId).Result.FirstOrDefault();
                var roles = await _userManager.GetRolesAsync(user);
                string role = roles.Contains("Admin") ? "Admin" : roles.FirstOrDefault() ?? "User";
                SessionUserDTO sessionUserDTO = new SessionUserDTO
                {
                    UserName = user.UserName,
                    UserId = userId,
                    ProfileId = _userProfile.GetByUserName(model.UserName).Result.ProfileId,
                    MappingId  = mappingId.MappingId,
                    Role=role
                };
                Helpers.SessionExtensions.SetObject(HttpContext.Session, "User", sessionUserDTO);
                //SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");

                
                if(roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //var ActiveCO = await _userProfile.GetByUserName(model.UserName);
                    //bool isCOActive = ActiveCO.IsActive;
                    int profileId = _userProfile.GetByUserName(model.UserName).Result.ProfileId;
                    var userMapping = _userMapping.GetByProfileId(profileId).Result.FirstOrDefault();
                    if (userMapping == null)
                    {
                        return Json(new { success = false, message = "User mapping not found." });
                    }
                    bool isCOActive = userMapping.IsActive;
                    if(!isCOActive)
                    {
                        HttpContext.Session.SetString("userActivate", "false");
                        return RedirectToAction("COContactUs", "Default");
                    }
                    HttpContext.Session.SetString("SAMLRole", "CO");
                }
                HttpContext.Session.SetString("UserGUID",_db.Users.FirstOrDefault(x=>x.UserName == model.UserName).Id.ToString());
                return RedirectToAction("Index", "Home");
            }
            else if(result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your account is locked out.");
            }
            else
            {
                TempData["UserName"] = model.UserName;
                //return RedirectToAction("Register", "Account", new { userName = model.UserName});
                return RedirectToAction("Register", "Account");
            }
            return View();
        }

        public async Task<IActionResult> Register(int id)
        {
            DTOuserProfile userProfileDTO = new DTOuserProfile();
            TempData.Keep("UserName");
            ///////GetUserProfile
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
                //return Json(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));   
                return View(signUpDto);
            }
         
        }

        public async Task<IActionResult> GetAllUsers(bool status)
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
                // _logger.LogError(1001, ex, "UserDB->GetAllUsersListPaginated");
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
            await _userMapping.Update(userMapping);
            return Json(new { success = true });
        }

    }
}
