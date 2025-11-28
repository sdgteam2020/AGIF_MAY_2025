using ClosedXML.Excel;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataTransferObject.Helpers;
using DataTransferObject.Identitytable;
using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneLogin.Saml;
using Org.BouncyCastle.Ocsp;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

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
            //Response.Redirect("https://iam2.army.mil/IAM/User", true);
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
                ArmyNo = profile.ArmyNo ?? string.Empty,
                name = profile.ProfileName ?? string.Empty,
            };

            Helpers.SessionExtensions.SetObject(HttpContext.Session, "User", sessionUserDTO);

            if (roles.Contains("LoanAdmin") || roles.Contains("ClaimAdmin") || roles.Contains("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (profile == null)
            {
                TempData["Message"] = "User mapping not found. Please contact administrator.";
                return RedirectToAction("Message", "Default");
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
                TempData["Message"] = "Your account has been locked due to multiple failed login attempts. Please try again later or contact the administrator.";
                return RedirectToAction("Message", "Default");
            }

            if (result.IsNotAllowed)
            {
                TempData["Message"] = "Your account is not allowed to sign in. Please contact administrator.";
                return RedirectToAction("Message", "Default");
            }

            var updatedFailedAttempts = await _userManager.GetAccessFailedCountAsync(user);
            var remainingAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts - updatedFailedAttempts;

            if (remainingAttempts > 0)
            {
                TempData["Message"] = $"Invalid username or password. {remainingAttempts} attempt(s) remaining before your account is locked.";
            }
            else
            {
                TempData["Message"] = "Your account has been locked due to multiple failed login attempts. Please contact the administrator.";
            }

            return RedirectToAction("Message", "Default");
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
                    DomainId = signUpDto.userName
                };

                var Result = await _userManager.CreateAsync(newUser, "Admin123!");
                if (!Result.Succeeded)
                {
                    return Json(Result.Errors);
                }
                await _userManager.AddToRoleAsync(newUser, "UnitCdr");

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

        [Authorize(Roles = "Admin,LoanAdmin")]
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

                var totalRecords = await query.CountAsync();

                query = ApplySearchFilter(query, request.searchValue);
                var filteredRecords = await query.CountAsync();

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
            EF.Functions.Like(x.DomainId ?? "", pattern) ||
                EF.Functions.Like(x.EmailId ?? "", pattern) ||
                EF.Functions.Like(x.MobileNo ?? "", pattern) ||
                EF.Functions.Like(x.ArmyNo ?? "", pattern) ||
                EF.Functions.Like(x.UnitName ?? "", pattern) ||
                EF.Functions.Like(x.AppointmentName ?? "", pattern) ||
                EF.Functions.Like(x.RegtName ?? "", pattern)
            );
        }
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
                var userList =  await queryableData.ToListAsync();
                //var userList = queryableData.ToList();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Users");

                    // Add headers and data to worksheet
                    worksheet.Cell(1, 1).Value = "S.No.";
                    worksheet.Cell(1, 2).Value = "User Name";
                    worksheet.Cell(1, 3).Value = "Army No";
                    worksheet.Cell(1, 4).Value = "Unit Cdr Name";
                    worksheet.Cell(1, 5).Value = "Appointment Name";
                    worksheet.Cell(1, 6).Value = "Unit Name";
                    worksheet.Cell(1, 7).Value = "Mobile No";
                    worksheet.Cell(1, 8).Value = "Email Id";
                    worksheet.Cell(1, 9).Value = "Regiment Name";
                    worksheet.Cell(1, 10).Value = "User Type";
                    worksheet.Cell(1, 11).Value = "Active Status";

                    int row = 2;
                    foreach (var user in userList)
                    {
                        worksheet.Cell(row, 1).Value = row - 1;
                        worksheet.Cell(row, 2).Value = user.DomainId;
                        worksheet.Cell(row, 3).Value = user.ArmyNo;
                        worksheet.Cell(row, 4).Value = user.ProfileName;
                        worksheet.Cell(row, 5).Value = user.AppointmentName;
                        worksheet.Cell(row, 6).Value = user.UnitName;
                        worksheet.Cell(row, 7).Value = user.MobileNo;
                        worksheet.Cell(row, 8).Value = user.EmailId;
                        worksheet.Cell(row, 9).Value = user.RegtName;
                        worksheet.Cell(row, 10).Value = user.IsPrimary? "Primary" : "Secondary";
                        worksheet.Cell(row, 11).Value = user.IsActive ? "Active" : "Inactive";
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
            if (!ModelState.IsValid)
            {
                return Json("Invalid request");
            }
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

            bool result = await _userProfile.SaveApprovedLogs(sessionUser.DomainId, ip, isActive, domainId, userProfile.ProfileId);

            userMapping.IsActive = isActive;
            userMapping.UpdatedOn = DateTime.Now;
            await _userMapping.Update(userMapping);

            return Json(new { success = true });
        }

        public async Task<IActionResult> FinalLogout()
        {
            //await _signInManager.SignOutAsync();
            //HttpContext.Session.Clear();
            //return View();
            // Remove the session token to clear user-specific session data
            HttpContext.Session.Remove("User");

            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                await _userManager.UpdateSecurityStampAsync(user); // invalidate all cookies
            }

            // Sign out the user from ASP.NET Identity authentication
            await _signInManager.SignOutAsync();

            //Clear server-side session state
            HttpContext.Session.Clear();

            // Delete session + auth cookies explicitly (good for audits)
            Response.Cookies.Delete(".AspNetCore.Identity.Application");
            Response.Cookies.Delete(".AspNetCore.Session");

            // Return the logout confirmation view to the user
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

        [HttpPost]
        public async Task<JsonResult> UpdateUserFormation(string domainId, bool isActive)
        {
            if (!ModelState.IsValid)
            {
                return Json("Invalid request");
            }
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


            userMapping.IsFmn = isActive;
            await _userMapping.Update(userMapping);

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

        //IAM Code begins
        //Login with Iam
        [AllowAnonymous]
        public async Task<IActionResult> UserLogin()
        {
           
            // return Redirect("/Identity/Account/login");
            System.String EncryptedResponse = Request.Form["SAMLResponse"];
           // return Json(EncryptedResponse);
           // string EncryptedResponse = "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";
            //string EncryptedResponse= "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";
            if (!string.IsNullOrEmpty(EncryptedResponse))
            {

                string decryptedsamlresponse = DecryptSAmlResponseNew(EncryptedResponse, "C:\\Cert\\App Certificate\\agif.army.mil.pfx", "Abc@2022");
                AccountSettings accountSettings = new AccountSettings();
                OneLogin.Saml.Response samlResponse = new OneLogin.Saml.Response(accountSettings);

                samlResponse.LoadXmlFromBase64(decryptedsamlresponse);

                if (samlResponse.IsValid_sign())
                {
                    Log log = new Log();
                    log.NameId = samlResponse.GetNameID();
                    log.SAMLRole = samlResponse.GetSAMLRole();
                    log.AppName = samlResponse.GetSAMLAppName();
                    log.DomainId = samlResponse.GetEntityID();
                    if(log.SAMLRole == "User" || string.IsNullOrEmpty(log.SAMLRole))
                    {
                        return RedirectToAction("Index", "Default");
                    }

                    if (log.NameId != null)
                    {
                        LoginViewModel model=new LoginViewModel();
                        model.UserName = log.NameId.ToUpper();
                        model.Password = "Admin123!";

                        var user = await GetUserAsync(model.UserName);
                        if (user == null)
                        {
                            ModelState.AddModelError(string.Empty, "Invalid username or password.");
                            TempData["UserName"] = model.UserName;
                            return RedirectToAction("Register", "Account");
                        }

                        if (await _userManager.IsLockedOutAsync(user))
                        {
                            return await HandleLockedOutUser(model, user);
                        }
                        var roles = await _userManager.GetRolesAsync(user);
                        string CurrentRole = roles[0].ToString().ToLower();
                        if (CurrentRole == "LoanAdmin".ToLower() || CurrentRole == "ClaimAdmin".ToLower())
                            CurrentRole = "Admin";

                        if (CurrentRole.ToLower() != log.SAMLRole.ToLower())
                        {
                            TempData["Message"] = "You are not  authorized to access this role.";
                            return RedirectToAction("Message", "Default");
                        }

                        var result = await SignInUserAsync(model, user);

                        if (result.Succeeded)
                        {
                            return await HandleSuccessfulLogin(user, model);
                        }

                        return await HandleFailedLogin(result, model, user);
                    }
                    else
                    {

                        Response.Redirect("https://iam2.army.mil/IAM/User", true);
                    }
                }
                else
                {

                    Response.Redirect("https://iam2.army.mil/IAM/User", true);
                }
            }
            else
            {
                Response.Redirect("https://iam2.army.mil/IAM/User", true);
            }
            return RedirectToAction("Index", "Home");

        }


        [AllowAnonymous]
        public string DecryptSAmlResponseNew(string Encryptedtext, string certificatepath, string password)
        {

            string result = "True";
            try
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes("alpha");

                System.String[] spearator = { Convert.ToBase64String(plainTextBytes) };

                // using the method
                System.String[] newstring = Encryptedtext.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                //string[] newstring = encryptedvalue.Split();
                string key = newstring[1].ToString();
                string plain = newstring[0].ToString();
                #region decryptkeyusingprivatekey
                try
                {
                    byte[] byteData = Convert.FromBase64String(key);
                    //   byte[] decryptedkey = new byte[16];
                    byte[] decryptedkey = new byte[32];
                    X509Certificate2 myCert2 = null;
                    RSACryptoServiceProvider rsa = null;

                    try
                    {
                        myCert2 = new X509Certificate2(@"C:\\Cert\\App Certificate\\agif.army.mil.pfx", "Abc@2022");
                        // rsa = (RSACryptoServiceProvider)myCert2.PrivateKey;
                        #region test
                        using (RSA rs = myCert2.GetRSAPrivateKey())
                        {
                            // rs.KeySize = 16;
                            decryptedkey = rs.Decrypt(byteData, RSAEncryptionPadding.Pkcs1);

                        }
                        #endregion
                    }
                    catch (Exception e)
                    {

                    }
                    // byte[] iv = new byte[16];
                    byte[] iv = new byte[32];


                    byte[] iv1 = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };


                    // result = DecryptString0705222_Final(plain, rsa.Decrypt(byteData, RSAEncryptionPadding.Pkcs1), iv1);
                    result = DecryptString0705222_Final(plain, decryptedkey, iv1);
                }
                catch (Exception exxx)
                {
                    result = exxx.Message;
                }
                #endregion

            }
            catch (Exception exx)
            {
                result = exx.Message;
            }

            return result;
        }
        [AllowAnonymous]
        private string DecryptString0705222_Final(string cipherText, byte[] key, byte[] iv)
        {
            // Instantiate a new Aes object to perform string symmetric encryption
            Aes encryptor = Aes.Create();

            encryptor.Mode = CipherMode.ECB;

            // Set key and IV
            //  byte[] aesKey = new byte[16];
            byte[] aesKey = new byte[32];
            //Array.Copy(key, 0, aesKey, 0, 16);
            Array.Copy(key, 0, aesKey, 0, 32);
            encryptor.Key = aesKey;
            encryptor.IV = iv;
            encryptor.Padding = PaddingMode.PKCS7;

            // Instantiate a new MemoryStream object to contain the encrypted bytes
            MemoryStream memoryStream = new MemoryStream();

            // Instantiate a new encryptor from our Aes object
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();

            // Instantiate a new CryptoStream object to process the data and write it to the 
            // memory stream
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);

            // Will contain decrypted plaintext
            string plainText = System.String.Empty;

            try
            {
                // Convert the ciphertext string into a byte array
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                // Decrypt the input ciphertext string
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);

                // Complete the decryption process
                cryptoStream.FlushFinalBlock();

                // Convert the decrypted data from a MemoryStream to a byte array
                byte[] plainBytes = memoryStream.ToArray();

                // Convert the decrypted byte array to string
                plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
            }
            catch (Exception exx)
            {

            }
            finally
            {
                // Close both the MemoryStream and the CryptoStream
                memoryStream.Close();
                cryptoStream.Close();
            }

            // Return the decrypted data as a string
            return plainText;

        }
        [AllowAnonymous]
        public void Logout()
        {
            if (Request.Form["SAMLResponse"].Count > 0)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(Request.Form["SAMLResponse"])))
                {
                    String EncryptedResponse = Convert.ToString(Request.Form["SAMLResponse"]);
                    if (!string.IsNullOrEmpty(EncryptedResponse))
                    {
                        AccountSettings accountSettings = new AccountSettings();
                        Response samlResponse = new Response(accountSettings);

                        string decryptedsamlresponse = DecryptSAmlResponseNew(EncryptedResponse, "C:\\Cert\\App Certificate\\agif.army.mil.pfx", "Abc@2022");
                        samlResponse.LoadXmlFromBase64(decryptedsamlresponse);



                        string nameid = string.Empty;
                        string issuer = string.Empty;
                        samlResponse.GetLogoutParameter(out nameid, out issuer);
                        HttpContext.Session.Clear();
                        try
                        {
                            // SendResponseToIAM("http://localhost:59474/Account/Logout", accountSettings.entityId, nameid);
                            SendResponseToIAM("https://agif.army.mil/Account/Logout", accountSettings.entityId, nameid);
                        }
                        catch (Exception exx)
                        {

                        }
                    }
                }
                else if (!string.IsNullOrEmpty(Convert.ToString(Request.Form["SAMLResponse"])))
                {
                    HttpContext.Session.Clear();

                    //Response.Redirect("http://localhost:59474/Account/FinalLogout");
                    Response.Redirect("https://agif.army.mil/Account/FinalLogout");
                }
                else
                {
                    AccountSettings acs = new AccountSettings();
                    SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
                    string NameId = dTOTempSession.Nameid;
                    string userRole = dTOTempSession.Role;
                    HttpContext.Session.Clear();
                    LogoutRequesttoIAM(userRole, acs.entityId, NameId);
                }
            }
            else
            {
                AccountSettings acs = new AccountSettings();
                SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
                string NameId = dTOTempSession.Nameid;
                string role = dTOTempSession.Role;

                //await HttpContext.SignOutAsync();
                HttpContext.Session.Clear();
                //HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);

                LogoutRequesttoIAM(role, acs.entityId, NameId);
            }
        }

        [AllowAnonymous]
        public void SendResponseToIAM(String issueurl, string entityid, string usernam)
        {
            AccountSettings accountSettings = new AccountSettings();

            OneLogin.Saml.AuthRequest req = new AuthRequest(new AppSettings(), accountSettings);

            //string ReuestXML = req.GetRequest(AuthRequest.AuthRequestFormat.Base64);
            //string ReuestXML = req.GetLogOutRequest(AuthRequest.AuthRequestFormat.Base64, issueurl, "https://iam2.army.mil/IAM/logout");
            string ReuestXML = req.GetLogOutRequest(AuthRequest.AuthRequestFormat.Base64, issueurl, "https://iam2.army.mil/IAM/logout");

            //Response.Redirect("https://iam2.army.mil/IAM/logout?SAMLResponse=" + ReuestXML);
            Response.Redirect("https://iam2.army.mil/IAM/logout?SAMLResponse=" + ReuestXML);

        }
        [AllowAnonymous]
        public void LogoutRequesttoIAM(String role, string entityid, string usernam)
        {
            AccountSettings accountSettings = new AccountSettings();
            OneLogin.Saml.AuthRequest req = new AuthRequest(new AppSettings(), accountSettings);

            string ReuestXML = req.SingleLogoutRequest(AuthRequest.AuthRequestFormat.Base64, entityid, role, usernam);
            //Response.Redirect("https://iam2.army.mil/IAM/singleAppLogout?SAMLRequest=" + HttpUtility.UrlEncode(ReuestXML), true);
            Response.Redirect("https://iam2.army.mil/IAM/singleAppLogout?SAMLRequest=" + HttpUtility.UrlEncode(ReuestXML), true);
        }
        public class Log
        {
            public string NameId { get; set; }
            public string SAMLRole { get; set; }

            public string AppName { get; set; }
            public string DomainId { get; set; }

        }

    }
}
