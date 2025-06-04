using DataAccessLayer;
using DataTransferObject.Identitytable;
using DataTransferObject.Model;
using DataTransferObject.Request;
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
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public AccountController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            HttpContext.Session.SetString("coId", "IC111111A"); // Clear the session variable for coId
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
                HttpContext.Session.SetString("DomainId", "TestDomain");
                var userEntity = await _db.Users.FirstOrDefaultAsync(i => i.UserName == model.UserName);
                var user2 = userEntity?.IntId.ToString();
                HttpContext.Session.SetString("UserIntId", user2); // Set the session variable for coId
            }
            else if(result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your account is locked out.");
            }
            else
            {
                return RedirectToAction("Register", "Account", new { userName = model.UserName});
            }
            return View();
        }

        public async Task<IActionResult> Register(string username, int? type, int? id)
        {
            var signup = new userProfileDTO();
            try
            {
                var coId = HttpContext.Session.GetString("coId");
                if (id == null && int.TryParse(coId, out int parsedId))
                {
                    id = parsedId;
                }

                if ((type == 2 && id.HasValue) || type == 3)
                {
                    var data = await _db.UserProfiles.FirstOrDefaultAsync(x => x.ProfileId == id);
                    if (data != null)
                    {
                        signup.Id = data.ProfileId;
                        signup.ArmyNo = data.ArmyNo;
                        signup.Name = data.Name;
                        signup.Email = data.Email;
                        signup.MobileNo = data.MobileNo;
                        //signup.UnitId = data.UnitId;
                        signup.ApptId = data.ApptId;
                        signup.rank = data.rank;
                        signup.Type = type ?? 0;

                        return View(signup);
                    }
                }

                signup.userName = username;
                signup.Type = type ?? 0;
            }
            catch (Exception)
            {
                // Consider logging the exception for debugging or production monitoring.
                throw;
            }

            return View(signup);
        }

        [HttpPost]
        public async Task<IActionResult> Register(userProfileDTO signUpDto)
        {
            int finalResult = 0;
            if (signUpDto.UnitId > 0)
            {
                if (signUpDto.Type == 2 || signUpDto.Type == 3)
                {
                    var coInfo = await _db.UserProfiles.FirstOrDefaultAsync(x =>  x.ArmyNo == signUpDto.ArmyNo);
                    var userInfo = await _db.UserMappings.FirstOrDefaultAsync(x => x.UnitId == signUpDto.UnitId);
                    if (coInfo != null)
                    {
                        bool isArmyNoChanged = coInfo.ArmyNo != signUpDto.ArmyNo;
                        coInfo.ArmyNo = signUpDto.ArmyNo;
                        coInfo.Name = signUpDto.Name;
                        coInfo.rank = signUpDto.rank;
                        coInfo.Email = signUpDto.Email;
                        coInfo.MobileNo = signUpDto.MobileNo;
                        //coInfo.UnitId = signUpDto.UnitId;
                        coInfo.ApptId = signUpDto.ApptId;


                        if (isArmyNoChanged)
                        {
                            coInfo.isActive = false;
                        }
                        var aspUser = await _userManager.FindByIdAsync(userInfo.UserID);
                        if (aspUser != null)
                        {
                            aspUser.Email = signUpDto.Email;
                            aspUser.PhoneNumber = signUpDto.MobileNo;

                            var updateResult = await _userManager.UpdateAsync(aspUser);
                            if (!updateResult.Succeeded)
                            {
                                return Json(updateResult.Errors);
                            }
                        }
                        if (_db.SaveChanges() > 0)
                        {
                            return Json(isArmyNoChanged ? 3 : 1);
                        }
                        return Json(2);

                    }
                }
                else
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

                    UserProfile model = new UserProfile
                    {
                        userName = signUpDto.userName,
                        ArmyNo = signUpDto.ArmyNo,
                        Name = signUpDto.Name,
                        Email = signUpDto.Email,
                        MobileNo = signUpDto.MobileNo,
                        rank = signUpDto.rank,
                        isActive = false,
                        regtCorps = signUpDto.regtCorps,
                        CreatedOn = DateTime.Now,
                        ApptId = signUpDto.ApptId,
                    };
                    _db.UserProfiles.Add(model);
                    await _db.SaveChangesAsync();
                    UserMapping userMapping = new UserMapping
                    {
                        UnitId = signUpDto.UnitId,
                        ProfileId = model.ProfileId,
                        UserID = newUser.Id.ToString(),
                        CreatedOn = DateTime.Now
                    };
                    _db.UserMappings.Add(userMapping);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("COContactUs", "Default");

                }
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("COContactUs", "Default");
        }
    }
}
