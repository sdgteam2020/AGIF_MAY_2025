using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataTransferObject.Helpers;
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
        private readonly IUserProfile _userProfile;
        private readonly IUserMapping _userMapping;
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
                var roles = await _userManager.GetRolesAsync(user);
                SessionUserDTO sessionUserDTO = new SessionUserDTO
                {
                    UserName = user.UserName,
                };
                Helpers.SessionExtensions.SetObject(HttpContext.Session, "User", sessionUserDTO);
                SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");

                
                if(roles.Contains("Admin"))
                {
                   return RedirectToAction("Index", "Home");
                }
                else
                {
                    var ActiveCO = await _db.UserProfiles.FirstOrDefaultAsync(x=>x.userName==model.UserName);
                    bool isCOActive = ActiveCO.isActive;
                    if(!isCOActive)
                    {
                        HttpContext.Session.SetString("userActivate", "false");
                        return RedirectToAction("COContactUs", "Default");
                    }
                    HttpContext.Session.SetString("SAMLRole", "unitCdr");
                }
                HttpContext.Session.SetString("UserGUID",_db.Users.FirstOrDefault(x=>x.UserName == model.UserName).Id.ToString());
                return RedirectToAction("Index", "Default");
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

        public async Task<IActionResult> Register(int id)
        {
            userProfileDTO userProfileDTO = new userProfileDTO();
           ///////GetUserProfile
            return View(userProfileDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Register(userProfileDTO signUpDto)
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
                var RoleRet = await _userManager.AddToRoleAsync(newUser, "UnitCdr");

                await _db.SaveChangesAsync();

                UserProfile userProfile = new UserProfile();
                await _userProfile.Add(userProfile);

                UserMapping userMapping = new UserMapping();
                await _userMapping.Add(userMapping);
                return RedirectToAction("COContactUs", "Default");
            }
            else
            {
                return Json(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));   

            }
         
        }
    }
}
