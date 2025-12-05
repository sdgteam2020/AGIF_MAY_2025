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

                var queryableData = _userProfile.GetAllUser(userStatus);
                var userList =  await queryableData.ToListAsync();

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
           
            System.String EncryptedResponse = Request.Form["SAMLResponse"];
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
            byte[] aesKey = new byte[32];
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

                HttpContext.Session.Clear();

                LogoutRequesttoIAM(role, acs.entityId, NameId);
            }
        }

        [AllowAnonymous]
        public void SendResponseToIAM(String issueurl, string entityid, string usernam)
        {
            AccountSettings accountSettings = new AccountSettings();

            OneLogin.Saml.AuthRequest req = new AuthRequest(new AppSettings(), accountSettings);

            string ReuestXML = req.GetLogOutRequest(AuthRequest.AuthRequestFormat.Base64, issueurl, "https://iam2.army.mil/IAM/logout");

            Response.Redirect("https://iam2.army.mil/IAM/logout?SAMLResponse=" + ReuestXML);

        }
        [AllowAnonymous]
        public void LogoutRequesttoIAM(String role, string entityid, string usernam)
        {
            AccountSettings accountSettings = new AccountSettings();
            OneLogin.Saml.AuthRequest req = new AuthRequest(new AppSettings(), accountSettings);

            string ReuestXML = req.SingleLogoutRequest(AuthRequest.AuthRequestFormat.Base64, entityid, role, usernam);
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
